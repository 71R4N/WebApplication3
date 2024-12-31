using System;
using System.Data;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Collections.Generic;
using System.IO;
using Microsoft.Data.SqlClient;

namespace lab
{
    class Program
    {

        public class dilerslist
        {
            public List<dilers> dilers { get; set; }
        }

        public class dilers
        {
            public string Name { get; set; }
            public string City { get; set; }
            public string Address { get; set; }
            public string Area { get; set; }
            public float Rating { get; set; }

        }

        public class carslist
        {
            public List<CarSalon> cars { get; set; }
        }

        public class CarSalon
        {
            public string firm { get; set; }
            public string model { get; set; }
            public int year { get; set; }
            public int power { get; set; }
            public string color { get; set; }
            public int price { get; set; }

        }

        static public void Main(string[] args)
        {
            var options = new JsonSerializerOptions()
            {
                NumberHandling = JsonNumberHandling.AllowReadingFromString |
                   JsonNumberHandling.WriteAsString
            };

            int k = 0;
            string jsonString = File.ReadAllText("C:\\Users\\Tom\\source\\repos\\WebApplication3\\WebApplication3\\cars.json");
            var cars = JsonSerializer.Deserialize<carslist>(jsonString, options);
            string jsonDiler = File.ReadAllText("C:\\Users\\Tom\\source\\repos\\WebApplication3\\WebApplication3\\dilers.json");
            var dilers = JsonSerializer.Deserialize<dilerslist>(jsonDiler, options);

            string connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=CarSalon;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

            // Создание подключения

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                Console.WriteLine("Подключение открыто");
                //Заполнение таблицы dilers
                using (var command = new SqlCommand("INSERT INTO diler (diler_id, Name, City, Address, Area, Rating) VALUES (@diler_id, @Name, @City, @Address, @Area, @Rating)", connection))
                {

                    foreach (var diler in dilers.dilers)
                    {
                        k++;
                        command.Parameters.Clear();
                        // Заполнение параметров запроса из объекта C#
                        command.Parameters.AddWithValue("@diler_id", k);
                        command.Parameters.AddWithValue("@Name", diler.Name ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@City", diler.City ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@Address", diler.Address ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@Area", diler.Area ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@Rating", diler.Rating);
                        command.ExecuteNonQuery();
                    }
                }
                //Заполнение таблицы cars
                using (var command = new SqlCommand(
                 "INSERT INTO car (firm, model, year, power, color, price, diler_id) VALUES (@firm, @model, @year, @power, @color, @price, @diler_id)", connection))
                {

                    foreach (var car in cars.cars)
                    {
                        // Получение случайного числа
                        Random rnd = new Random();

                        command.Parameters.Clear();

                        // Заполнение параметров запроса из объекта C#
                        command.Parameters.AddWithValue("@firm", car.firm ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@model", car.model ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@year", car.year);
                        command.Parameters.AddWithValue("@power", car.power);
                        command.Parameters.AddWithValue("@color", car.color ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@price", car.price);
                        int rndValue = rnd.Next(1, dilers.dilers.Count);
                        command.Parameters.AddWithValue("@diler_id", rndValue);

                        // Выполнение запроса
                        command.ExecuteNonQuery();
                    }
                }
                connection.Close();
            }
            Console.WriteLine("Подключение закрыто...");

        }
    }
}