using Microsoft.Data.SqlClient;
using System.Configuration;
using System.Data.SqlClient;    
using System.Diagnostics;
using System.Linq.Expressions;



/*
* user Manual
* 1. Функции:
* 1.1 CreateGood создает строку в Базе данных. На вход 1-Название товара(string) 2-Описание товара(string)
* 3-цена (double) 4-Продавец(string(имя в тг через @)) 5-теги (string(Все с маленькой буквы, псоле каждого слова ","
* например string  tags = "техника,л,г-2,")
* 1.2 GetGoodById Возвращает массив объектов в котором 6 элементов: 1(0)-id товара 2(1)-Название товара 3(2)-Описание товара 
* 4(3)-цена 5(4)-Имя продовца( тг имя через @) 6(5)-Теги(Категория,Корпус Вуза, Корпус Общаги)
* 1.3  AddTag добавляет тег. На вход айди товара
* 1.4  AddTags добавляет несколбко тегов. 
* На вход айди и строка тегов(такая же как при создании строки, НО без запятой перед последним словом)
* 1.5  RemoveTags удаляет все теги. На вход айди товара
* 1.6  GetTagsArrayForGood Выводит массив строк в уотором написаны все теги по определенному айди. На вход айди товара
* 1.7  DeleteGoodById Полностью удаляет строку по айди. На вход айди товара
* 1.8  FindGoodsByTag Возвращает лист айди товаров, в которых есть  тего которые даются на вход. 
* На вход строка тегов(Такая-же как при создании)
* 1.9  FindGoodsBySeller Возвращает лист в котором айди товаров, которые вытсавил продавец. На вход имя продавца(как при создании) 
* 2. Доступ к БД:
* Тебе надо открыть БД, потом свойства и там будет парамет "Строка подключения" 
* копируешь то что там написано. 
* Открываешь App.config  там будет connectionString вмсето того что там написано вставляешь скопированную строку
*/

namespace avitomisisDB
{
    public class DB
    {

        private static string connectionString = ConfigurationManager.ConnectionStrings["GoodsDB"].ConnectionString;

        public static int CreateGood(string gname, string description,  double price, string seller, string[] tags, string photo)
        {

            int ret = 0;
            // Создание подключения
            SqlConnection connection = new SqlConnection(connectionString);
            try
            {
                // Открываем подключение
                connection.Open();
                //Console.WriteLine("Подключение открыто");

                // здесь делаем полезное
                string createGoodsExpression = "INSERT INTO Goods (gname, description, cost, seller, photo) " +
                    "VALUES (N'" + gname + "', " +
                    "N'" + description + "', " +
                    "'" + price.ToString().Replace(",",".") + "'," +
                    "'" + seller + "',"+
                    "'" +photo+"') SELECT SCOPE_IDENTITY()";

                //Console.WriteLine(createGoodsExpression);

                SqlCommand command = new SqlCommand(createGoodsExpression, connection);
                ret = Convert.ToInt32(command.ExecuteScalar());
                AddTags(ret, tags);


            }
            catch (SqlException ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                // закрываем подключение
                connection.Close();

                //Console.WriteLine("Подключение закрыто...");
            }

            return ret;
        }

        public static object[] GetGoodById(int gooodID)
        {
            object[] ret = new object[6];
            // Создание подключения
            SqlConnection connection = new SqlConnection(connectionString);
            try
            {
                // Открываем подключение
                connection.Open();
                //Console.WriteLine("Подключение открыто");

                // здесь делаем полезное
                string expression = "SELECT * FROM Goods Where id=" + gooodID;

                SqlCommand command = new SqlCommand(expression, connection);
                command.ExecuteScalar();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        ret[0] = reader["id"];
                        ret[1] = reader["gname"];
                        ret[2] = reader["description"];
                        ret[3] = reader["cost"];
                        ret[4] = reader["seller"];
                        ret[5] = reader["tags"];
                    }
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                // закрываем подключение
                connection.Close();
                //Console.WriteLine("Подключение закрыто...");
            }
            return ret;
        }

        public static void AddTag(int goodID, string tag)
        {

            // Создание подключения
            SqlConnection connection = new SqlConnection(connectionString);
            try
            {
                // Открываем подключение
                connection.Open();
                //Console.WriteLine("Подключение открыто");

                // здесь делаем полезное
                string tags = "";
                string expression = "SELECT tags FROM Goods Where id=" + goodID;
                SqlCommand command = new SqlCommand(expression, connection);
                command.ExecuteScalar();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        if (!reader.IsDBNull(0))
                        {
                            tags = reader.GetString(0);
                        }


                    }

                }

                tags = tags + tag + ",";

                string updateExprassion = "UPDATE Goods SET tags=N'" + tags + "' WHERE id=" + goodID;
                SqlCommand updateCommand = new SqlCommand(updateExprassion, connection);
                updateCommand.ExecuteNonQuery();


            }
            catch (SqlException ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                // закрываем подключение
                connection.Close();
                //Console.WriteLine("Подключение закрыто...");
            }

        }
        public static void AddTags(int goodID, string[] tags)
        {

            for (int i = 0; i < tags.Length; i++)
            {
                AddTag(goodID, tags[i]);
            }
        }
        public static void RemoveTags(int goodID)
        {


    // Создание подключения
    SqlConnection connection = new SqlConnection(connectionString);
            try
            {
                // Открываем подключение
                connection.Open();
                //Console.WriteLine("Подключение открыто");

                // здесь делаем полезное
                string tags = "NULL";
                string expression = "UPDATE Goods SET tags=N'" + tags + "' Where id=" + goodID;
                SqlCommand command = new SqlCommand(expression, connection);
                command.ExecuteNonQuery();

            }
            catch (SqlException ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                // закрываем подключение
                connection.Close();
                //Console.WriteLine("Подключение закрыто...");
            }

        }
        public static string[] GetTagsArrayForGood(int goodID)
        {
            List<string> tags = new List<string>();
            string stTags = "";

            // Создание подключения
            SqlConnection connection = new SqlConnection(connectionString);
            try
            {

                // Открываем подключение
                connection.Open();
                //Console.WriteLine("Подключение открыто");

                // здесь делаем полезное

                string expression = "SELECT tags FROM Goods Where id=" + goodID;
                SqlCommand command = new SqlCommand(expression, connection);
                command.ExecuteScalar();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        tags.Add(reader.GetString(0));
                    }

                }


            }

            catch (SqlException ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                // закрываем подключение
                connection.Close();
                //Console.WriteLine("Подключение закрыто...");
            }
            stTags += tags[0].ToString();
            string[] arrTags = stTags.Split(",");

            return arrTags;
        }
        public static void DeleteGoodById(int goodId)
        {
            int ret = 0;
            // Создание подключения
            SqlConnection connection = new SqlConnection(connectionString);
            try
            {
                // Открываем подключение
                connection.Open();
                //Console.WriteLine("Подключение открыто");

                // здесь делаем полезное
                string deleteGoodsExpression = "DELETE FROM Goods Where id=" + goodId;
                //Console.WriteLine(createGoodsExpression);

                SqlCommand command = new SqlCommand(deleteGoodsExpression, connection);
                ret = Convert.ToInt32(command.ExecuteScalar());
                Console.WriteLine("Удален товар с id={0}", goodId);

            }
            catch (SqlException ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                // закрываем подключение
                connection.Close();
                //Console.WriteLine("Подключение закрыто...");
            }

        }
        public static List<int> FindGoodsByTag(string tag)
        {

            List<int> ret = new List<int>();
            // Создание подключения
            SqlConnection connection = new SqlConnection(connectionString);
            try
            {


                // Открываем подключение
                connection.Open();
                //Console.WriteLine("Подключение открыто");

                // здесь делаем полезное
                string expression = "SELECT id FROM Goods Where tags LIKE N'%" + tag + "%'";

                SqlCommand command = new SqlCommand(expression, connection);
                command.ExecuteScalar();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ret.Add(reader.GetInt32(0));
                    }
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                // закрываем подключение
                connection.Close();
                //Console.WriteLine("Подключение закрыто...");
            }
            return ret;
        }
        public static List<int> FindGoodsByTags(string[] tag)
        {
            List<int> ret = new List<int>();
            List<int>[] allLists = new List<int>[tag.Length];
            for (int i = 0; i < allLists.Length; i++)
            {
                allLists[i] = FindGoodsByTag(tag[i]);
            }



    for (int i = 0; i < allLists[0].Count; i++)
            {
                int currentId = allLists[0][i];
                bool existsInAllLists = true;
                for (int j = 1; j < allLists.Length; j++)
                {
                    if (allLists[j].IndexOf(currentId) < 0)
                    {
                        existsInAllLists = false;
                        break;
                    }
                }
                if (existsInAllLists)
                {
                    ret.Add(currentId);
                }
            }
            return ret;

        }
        public static List<int> FindGoodsBySeller(string seller)
        {
            List<int> ret = new List<int>();
            // Создание подключения
            SqlConnection connection = new SqlConnection(connectionString);
            try
            {
                // Открываем подключение
                connection.Open();
                //Console.WriteLine("Подключение открыто");

                // здесь делаем полезное
                string expression = "SELECT id FROM Goods Where seller=N'" + seller + "'";

                SqlCommand command = new SqlCommand(expression, connection);
                command.ExecuteScalar();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ret.Add(reader.GetInt32(0));

                    }
                }


            }
            catch (SqlException ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                // закрываем подключение
                connection.Close();
                //Console.WriteLine("Подключение закрыто...");
            }
            return ret;
        }


    }
}