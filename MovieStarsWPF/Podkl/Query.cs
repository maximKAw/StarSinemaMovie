using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json; //для работы с JSON
using System.Data.SqlClient;
using System.Data;       
using System.Windows;

namespace MovieStarsWPF.Podkl
{
    //Описание класса "ClassStr"
    public class ClassStr
    {
        public static string TekPrava = "";
        public static int TekSpravochnik = 0;

        public static int TekCodArtist = 0;
        public static string TekFamNameArtist = "";

        public static int TekCodFilm = 0;
        public static string TekNameFileFoto = "";
        public static string TekNameFilm = "";
    }

    //Описание класса "ClassArtist"
    public class ClassArtist
    {
        public int CodArtist { get; set; }
        public string Fam { get; set; }
        public string Name { get; set; }
        public string Otch { get; set; }
        public string StrFoto { get; set; }

        public ClassArtist() { }

        public ClassArtist(int _CodArtist, string _Fam, string _Name, string _Otch, string strFoto)
        {
            CodArtist = _CodArtist;
            Fam = _Fam;
            Name = _Name;
            Otch = _Otch;
            StrFoto = strFoto;
        }
    }

    //Описание класса "ClassFilm"
    public class ClassFilm
    {
        public int CodFilm { get; set; }
        public string Name { get; set; }
        public string StrFoto { get; set; }
        public int CodArtist { get; set; }

        public ClassFilm() { }

        public ClassFilm(int _CodFilm, string _Name, string strFoto, int _CodArtist)
        {
            CodFilm = _CodFilm;
            Name = _Name;
            StrFoto = strFoto;
            CodArtist = _CodArtist;
        }
    }

    //Описание класса "ClassNagrada"
    public class ClassNagrada
    {
        public int CodNagrada { get; set; }
        public string Name { get; set; }
        public int CodArtist { get; set; }

        public ClassNagrada() { }

        public ClassNagrada(int _CodNagrada, string _Name, int _CodArtist)
        {
            CodNagrada = _CodNagrada;
            Name = _Name;
            CodArtist = _CodArtist;
        }
    }

    //Описание класса "ClassBiography"
    public class ClassBiography
    {
        public int CodBiography { get; set; }
        public DateTime DataR { get; set; }
        public string FIO_Mother { get; set; }
        public string FIO_Father { get; set; }
        public string Country { get; set; }
        public int Rost { get; set; }
        public string Pol { get; set; }
        public string Inform { get; set; }
        public int CodArtist { get; set; }

        public ClassBiography() { }

        public ClassBiography(int _CodBiography,  DateTime _DataR, string _FIO_Mother, string _FIO_Father,
                              string _Country,    int _Rost,       string _Pol,        string _Inform, 
                              int _CodArtist)
        {
            CodBiography = _CodBiography;
            DataR = _DataR;
            FIO_Mother = _FIO_Mother;
            FIO_Father = _FIO_Father;
            Country = _Country;
            Rost = _Rost;
            Pol = _Pol;
            Inform = _Inform;
            CodArtist = _CodArtist;
        }
    }



    internal class Query
    {
        SqlConnection connection;  //создать подключение между нашим проектом и БД
        SqlCommand command;        //нужен для выполнения операция вставки, редактирования и удаления данных БД
        SqlDataAdapter datadapter; //нужен для получения набора данных из БД
        DataTable bufferTable;

        //Конструктор класса
        public Query(string Conn) //Передаем строку подключения
        {
            //Инициализация
            connection = new SqlConnection(Conn);
            bufferTable = new DataTable();
        }

        //Метод для получения данных из таблицы
        public DataTable GetDataFromTable(string zapros)
        {
            try
            {
                connection.Open();  //Открыть соединение
                                    //Инициализируем - передаем запрос и объект класса oledbconnection - connection
                datadapter = new SqlDataAdapter(zapros, connection);
                bufferTable.Clear();  //Очищаем буфер запроса
                datadapter.Fill(bufferTable); //вызываем метод Fill, чтобы после запроса заполнил буферную таблицу данными 
                connection.Close();  //Закрываем соединение
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return bufferTable;  //Возвращаем результат
        }

        //Метод для удаления данных во всех таблицах
        public void DeleteFromTable(string NameTabl, string PoleTable, int ID)
        {
            string zapros = "";

            try
            {
                connection.Open();
                zapros = "delete from " + NameTabl + " where " + PoleTable + " = " + Convert.ToString(ID);
                command = new SqlCommand(zapros, connection);
                command.ExecuteNonQuery();
                connection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //Метод для обновления данных в таблице
        public void Update_JSON(string NameTabl, string strData)
        {
            string zapros = "";

            switch (NameTabl)
            {
                case "Table_Artist":
                    ClassArtist PerArtist = JsonConvert.DeserializeObject<ClassArtist>(strData);
                    zapros = "update " + NameTabl + " set " +
                             "Fam = '" + PerArtist.Fam +
                             "', Name = '" + PerArtist.Name +
                             "', Otch = '" + PerArtist.Otch +
                             "', StrFoto = '" + PerArtist.StrFoto +
                             "' where CodArtist = " + PerArtist.CodArtist;
                    break;
                case "Table_Film":
                    ClassFilm PerFilm = JsonConvert.DeserializeObject<ClassFilm>(strData);
                    zapros = "update " + NameTabl + " set " +
                             "Name= '" + PerFilm.Name +
                             "', StrFoto = '" + PerFilm.StrFoto +
                             "', CodArtist= " + PerFilm.CodArtist +
                             " where CodFilm = " + PerFilm.CodFilm;
                    break;
                case "Table_Nagrada":
                    ClassNagrada PerNagrada = JsonConvert.DeserializeObject<ClassNagrada>(strData);
                    zapros = "update " + NameTabl + " set " +
                             "Name= '" + PerNagrada.Name +
                             "', CodArtist= " + PerNagrada.CodArtist +
                             " where CodNagrada = " + PerNagrada.CodNagrada;
                    break;
                case "Table_Biography":
                    ClassBiography PerBiography = JsonConvert.DeserializeObject<ClassBiography>(strData);
                    zapros = "update " + NameTabl + " set " +
                             "DataR= '" + PerBiography.DataR.ToString("yyyyMMdd") +
                             "', FIO_Mother= '" + PerBiography.FIO_Mother +
                             "', FIO_Father= '" + PerBiography.FIO_Father +
                             "', Country= '" + PerBiography.Country +
                             "', Rost= " + PerBiography.Rost +
                             ", Pol= '" + PerBiography.Pol +
                             "', Inform= '" + PerBiography.Inform +
                             "', CodArtist= " + PerBiography.CodArtist +
                             " where CodBiography = " + PerBiography.CodBiography;
                    break;
            }


            connection.Open();  //Открыть соединение

            //Инициализируем - передаем запрос и объект класса oledbconnection - connection
            command = new SqlCommand(zapros, connection);
            command.ExecuteNonQuery(); //Выполняем запрос. В качестве результата возвращает кол-во обработанных строк
            connection.Close(); //Закрываем соединение
        }

        //Метод для обновления данных в таблице
        public int Add_JSON(string NameTabl, string strData)
        {
            string zapros = "";

            switch (NameTabl)
            {
                case "Table_Artist":
                    ClassArtist PerArtist = JsonConvert.DeserializeObject<ClassArtist>(strData);
                    zapros = "insert into " + NameTabl + "(Fam,Name,Otch,StrFoto) values ('" +
                             PerArtist.Fam + "','" +
                             PerArtist.Name + "','" +
                             PerArtist.Otch + "','" +
                             PerArtist.StrFoto + "')";
                    break;
                case "Table_Film":
                    ClassFilm PerFilm = JsonConvert.DeserializeObject<ClassFilm>(strData);
                    zapros = "insert into " + NameTabl + "(Name,StrFoto,CodArtist) values ('" +
                             PerFilm.Name + "','" +
                             PerFilm.StrFoto + "'," +
                             PerFilm.CodArtist + ")";
                    break;
                case "Table_Nagrada":
                    ClassNagrada PerNagrada = JsonConvert.DeserializeObject<ClassNagrada>(strData);
                    zapros = "insert into " + NameTabl + "(Name,CodArtist) values ('" +
                             PerNagrada.Name + "'," +
                             PerNagrada.CodArtist + ")";
                    break;
                case "Table_Biography":
                    ClassBiography PerBiography = JsonConvert.DeserializeObject<ClassBiography>(strData);
                    zapros = "insert into " + NameTabl + "(DataR,FIO_Mother,FIO_Father,Country,Rost,Pol,Inform,CodArtist) values ('" +
                             PerBiography.DataR.ToString("yyyyMMdd") + "','" +
                             PerBiography.FIO_Mother + "','" +
                             PerBiography.FIO_Father + "','" +
                             PerBiography.Country + "'," +
                             PerBiography.Rost + ",'" +
                             PerBiography.Pol + "','" +
                             PerBiography.Inform + "'," +
                             PerBiography.CodArtist + ")";
                    break;
            }

            connection.Open();  //Открыть соединение
            command = new SqlCommand(zapros, connection);
            command.ExecuteNonQuery(); //Выполняем запрос. В качестве результата возвращает кол-во обработанных строк

            //Получаем ID добавленной записи
            command = new SqlCommand($"Select @@Identity", connection);
            int id = Convert.ToInt32(command.ExecuteScalar());
            connection.Close(); //Закрываем соединение
            return id; //Возвращаем ID добавленной записи
        }
    }
}
