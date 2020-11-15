using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

//Saving2 kendimizin yazdigi Saving ise direk udemy icinden aldigmiz
namespace RpgCoreCombatGame.Saving2
{
    //SavingSystem icinde Save ve Load olan methodlari bulundururuz
    public class SavingSystem : MonoBehaviour
    {
        [SerializeField] float waitForSecondLoad = 1f;

        //SavingWrapper icinde cagiripyorzu start method'u icinde
        public IEnumerator LoadLastScene(string saveFile)
        {
            Dictionary<string, object> states = LoadFile(saveFile);
            int buildIndex = SceneManager.GetActiveScene().buildIndex;
            if (states.ContainsKey("lastSceneIndex"))
            {
                buildIndex = (int)states["lastSceneIndex"];
            }

            yield return SceneManager.LoadSceneAsync(buildIndex);

            //yield return new WaitForSeconds(waitForSecondLoad);

            RestoreState(states);
        }

        public void Save(string saveFile)
        {
            //string path = GetPathFromSaveFile(saveFile);
            //Debug.Log("Saving to " + path);

            //bir dosya actiysak onu kapatmamiz lazimdir fileSteam.Close() methodu da olur yada using ile direk isi bittikten sonra GarbageCollector gonderebillriz
            //using (FileStream fileStream = File.Open(path, FileMode.Create))
            //{
            //FileMode.Create eger dosya boyle bir dosya yok ise o dosyayi oluturucak var ise o dosyanın ustune yazicak

            //bu yapi sayesinde biz Encoding ile UTF8 ile Hello World'u binary(0 ve 1) yapisina cevirip bunu byte dizisine atiyoruz
            //byte[] bytes = System.Text.Encoding.UTF8.GetBytes("!Hello World");
            //fileStream.Write(bytes, 0, bytes.Length); //burda da byte dizisini dosyamiza yazdiriyoruz
            //Transform playerTransform = GetPlayerTransform();

            //bu sekil calistiridigmizda bize hata verir ve Vector3 serilize edemediginin bilgisini verir bunu icin bizde araya bir class atariz Model mantiginda SerializableVector3 diye burda serilize ve deserilize islemlerini burda yapariz
            //BinaryFormatter formatter = new BinaryFormatter();
            //SerializableVector3 serializableVector3 = new SerializableVector3(playerTransform.position);
            //formatter.Serialize(fileStream, CaptureState());


            //BinaryFormatter yazdiktan sorna buffer ihtiyacimiz kalmadi buffer ile biz manuel serilize islemi yaptik ve binary'ye cevirdik BinaryFormatter ile bu isi daha kisa yapmis olduk
            //byte[] buffer = SerializeVector(playerTransform.position);
            //fileStream.Write(buffer, 0, buffer.Length);

            //Close yerine using kullanirsak garbage collector sayesinde fileStream kapanmis olucaktir
            //fileStream.Close(); //mutlaka actigimiz doysayi kapamamiz gerekmektedir
            //}

            Dictionary<string, object> state = LoadFile(saveFile);

            CaptureState(state);

            SaveFile(saveFile, state);
        }

        public void Load(string saveFile)
        {
            //string path = GetPathFromSaveFile(saveFile);
            //Debug.Log("Loading from " + path);

            //using (FileStream fileStream = File.Open(path, FileMode.Open))
            //{
            //BinaryFormatter.Deserialize kullanildigi icin artik bu yapiya ihtiyac duymuyoruz
            //buffer arrayini uzunlugunu fileStream.Length kadar verdik
            //byte[] buffer = new byte[fileStream.Length]; //once byte arrayi oljusturduk fileStream iciideki Read method'u calistirabilemk icin
            //fileStream.Read(buffer, 0, buffer.Length); //Read ile yapmazsak sadece buffur ile yaparsak bizim vector 0 nokatasina isinlar ama read ile son save'i okuyabliriz olustrudmuzu buffer'in icini Read ile doldururuz

            //Transform playerTransform = GetPlayerTransform();
            //Debug.Log(System.Text.Encoding.UTF8.GetString(buffer));
            //playerTransform.GetComponent<NavMeshAgent>().enabled = false;
            //playerTransform.transform.position = DeserializeVector(buffer);
            //playerTransform.GetComponent<NavMeshAgent>().enabled = true;

            //BinaryFormatter binaryFormatter = new BinaryFormatter();
            //RestoreState(binaryFormatter.Deserialize(fileStream));

            //if (serializableVector3 != null)
            //{
            //NavMeshAgent navMeshAgent = playerTransform.GetComponent<NavMeshAgent>();
            //navMeshAgent.enabled = false;
            //playerTransform.position = serializableVector3.ToVector3();
            //navMeshAgent.enabled = true;  
            //}
            //}

            RestoreState(LoadFile(saveFile));
        }

        public void Delete(string saveFile)
        {
            string path = GetPathFromSaveFile(saveFile);
            
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            else
            {
                Debug.LogError("File do not exists");
            }
        }

        //save method'u daha duzgun yazmak icin method'lastridik
        private void SaveFile(string saveFile, object state)
        {
            string path = GetPathFromSaveFile(saveFile);

            using (FileStream fileStream = File.Open(path, FileMode.Create))
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                binaryFormatter.Serialize(fileStream, state);
            }
        }

        //ayni islemi Load icinde yapmis olduk
        private Dictionary<string,object> LoadFile(string saveFile)
        {
            string path = GetPathFromSaveFile(saveFile);

            Debug.Log(path);

            //eger path yok ise bizim icin new dictionary donucek
            if (!File.Exists(path))
            {
                return new Dictionary<string, object>();
            }

            using (FileStream fileStream = File.Open(path, FileMode.Open))
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                return binaryFormatter.Deserialize(fileStream) as Dictionary<string, object>;
            }
        }

        //RestoreState ise Load Method'u icinde kullanilir fileStream icindeki butun dosyalari tek tek restore edicek yani yerine vericek
        private void RestoreState(Dictionary<string, object> state) //state fileStream icinden geliyor Load iicinde Deserialize'dan sonra bu islem calisiyor
        {
            foreach (SaveableEntity item in FindObjectsOfType<SaveableEntity>())
            {
                string id = item.GetUniqueIdentifier();

                if (state.ContainsKey(id))
                {
                    item.RestoreState(state[id]);
                }
            }
        }

        //Save method'u icinde kullanriiz geleen butun SavealeEntitiy kapsayanlari ele geciricek method'um
        //bu method bizim once dictionary yapisini olusturdugmuz ve butun saveable olanlari listeledigmiz yapimizdir item yani SaveableEntity olan nesnerimi dongu icinde kullarinzi tek tek dictionary iicne atariz once string ref icinde item yani saveableentity iicnden bir getuniqueidentifier cagiririz sonra item.CaptureState ile vector3'leri gonderirirzz
        private void CaptureState(Dictionary<string, object> state)
        {
            //SaveableEntity component icinde bizim save ediceklerimizi belierler ve bu method iicnde bize dictionary icin once unique bir id cikardik ve SaveableEntiity icinden gelen capturestate dictionary icinde atariz
            foreach (SaveableEntity item in FindObjectsOfType<SaveableEntity>())
            {
                state[item.GetUniqueIdentifier()] = item.CaptureState();
            }

            //en son hangi scene kaldigmizi save edebilmek icin bu yapidan yararlandik
            state["lastSceneIndex"] = SceneManager.GetActiveScene().buildIndex;
        }

        #region Binary Manual cevirme yolu BinaryFormatter'dan bunun iicndeki yapilara gerek kalmadi
        //private Transform GetPlayerTransform()
        //{
        //    return GameObject.FindWithTag("Player").transform;
        //}

        ////byte[] yapmamziin nedeni binary cevirip yani serilize edip oyle dosyalamak istiyoruz
        //private byte[] SerializeVector(Vector3 vector3)
        //{
        //    byte[] vectorByte = new byte[3 * 4]; //3 tane deger atacigimiz icin 3 satir yaptik 4 sutun yaptik 0 , 4, 8 bu yuzden yazdik
        //    BitConverter.GetBytes(vector3.x).CopyTo(vectorByte, 0);
        //    BitConverter.GetBytes(vector3.y).CopyTo(vectorByte, 4);
        //    BitConverter.GetBytes(vector3.z).CopyTo(vectorByte, 8);

        //    return vectorByte;
        //}

        ////dosya iicinde serilize olan binary code'unu Vector3 veriyoruz
        //private Vector3 DeserializeVector(byte[] buffer)
        //{
        //    Vector3 vector3 = new Vector3();
        //    vector3.x = BitConverter.ToSingle(buffer, 0);
        //    vector3.y = BitConverter.ToSingle(buffer, 4);
        //    vector3.z = BitConverter.ToSingle(buffer, 8);

        //    return vector3;
        //} 
        #endregion

        //bu method sayesinde hem bu dosyanin yerini \save yazdirmis olduk save.sav ise dosyaya uzanti vermek icin kullandik
        private string GetPathFromSaveFile(string saveFile)
        {
            //persistenDaataPath bize bu dosya yolunu donucektir
            //return string.Format(@"{0}\{1}.sav", Application.persistentDataPath, saveFile);

            //bu yontem Path bize System.IO dll'den gelen bir class'dir Combine ise birletirdi dosya yolu ise save file adini
            return Path.Combine(Application.persistentDataPath, saveFile + ".sav");
        }
    }
}
