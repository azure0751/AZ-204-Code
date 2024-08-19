using DataStoreInsertApp.Models;
using Microsoft.Data.SqlClient;

namespace DataStoreInsertApp.DataAccess
{
    public class PersonDAL
    {
        private readonly string _connectionString;

        public PersonDAL(IConfiguration configuration)
        {
            _connectionString = configuration["azuresqldbConnection"];// connectionString;
        }

        public void InsertPerson(Person person)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("INSERT INTO Persons (Name, Email, Age) VALUES (@Name, @Email, @Age)", con);
                cmd.Parameters.AddWithValue("@Name", person.Name);
                cmd.Parameters.AddWithValue("@Email", person.Email);
                cmd.Parameters.AddWithValue("@Age", person.Age);

                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public List<Person> GetAllPersons()
        {
            var persons = new List<Person>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string sql = "SELECT Id, Name, Email, Age FROM Persons";
                SqlCommand command = new SqlCommand(sql, connection);
                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var person = new Person
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1),
                            Email = reader.GetString(2),
                            Age = reader.GetInt32(3)
                        };
                        persons.Add(person);
                    }
                }
            }

            return persons;
        }
    }
}
