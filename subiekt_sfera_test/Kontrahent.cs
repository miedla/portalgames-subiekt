using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace subiekt_sfera_test
{
    public class Kontrahent
    {
        public string IdZakupu { get; set; }

        public string Id { get; set; }

        public string DatabaseType { get; set; }

        public string Ulica { get; set; }

        public string Miasto { get; set; }

        public string Zip { get; set; }

        public string PanstwoKod { get; set; }

        public string PanstwoNazwa { get; set; }

        public int PanstwoId { get; set; }

        public string Nazwa { get; set; }

        public string Imie { get; set; }

        public string Nazwisko { get; set; }

        public string Fima { get; set; }

        public string MiastoKod { get; set; }

        public string NumerZamowienia { get; set; }

        public string NIP { get; set; }

        public string Email { get; set; }

        public string NazwaFirmy { get; set; }

        public string KodPocztowyFirma { get; set; }

        public string AdresFirma { get; set; }

        public string MiastoFirma { get; set; }

        public Zamowienie Zamowienie { get; set; }

        public Kontrahent()
        {
            Zamowienie = new Zamowienie();
        }

        /// <summary>
        /// Publiczna funkcja pozwalająca dodać płatność do zamówienia kontrahenta
        /// </summary>
        public void DodajPlatnoscDoPortalgames()
        {
            MySqlConnectionStringBuilder portalgamesconn = Utils.PortalGamesConnSring;
            using (MySqlConnection conn = new MySqlConnection(portalgamesconn.ToString()))
            {
                conn.Open();
                string sqlCommand = "INSERT INTO `order_pay_history`(order_id, add_date, pay_date, user_id, customer_id, price_brutto) VALUES(@orderId, @addDate, @payDate, @userId, @customerId, @priceBrutto);";
                using (var cmd = conn.CreateCommand())
                {
                    cmd.Parameters.AddWithValue("orderId", Zamowienie.IdZamowienia);
                    cmd.Parameters.AddWithValue("addDate", DateTime.Now.Date);
                    cmd.Parameters.AddWithValue("payDate", DateTime.Now.Date);
                    cmd.Parameters.AddWithValue("userId", 3);
                    cmd.Parameters.AddWithValue("customerId", 0);
                    cmd.Parameters.AddWithValue("priceBrutto", Zamowienie.KwotaDoZaplaty);
                    cmd.CommandText = sqlCommand;

                    cmd.ExecuteNonQuery();
                }
                conn.Close();
            }
        }

        /// <summary>
        /// Publiczna funkcja pobierająca identyfikator państwa z Subiekta
        /// </summary>
        /// <param name="panstwo">Nazwa państwa, z którego pochodzi kontrahent</param>
        /// <returns></returns>
        public int PobierzIdPanstwa(string panstwo)
        {
            SqlConnectionStringBuilder subiektConn = new SqlConnectionStringBuilder();

            subiektConn.UserID = ConfigConnection.UzytkownikProperty;
            subiektConn.Password = ConfigConnection.UzytkownikHasloProperty;
            subiektConn.InitialCatalog = ConfigConnection.BazaGtProperty;
            subiektConn.DataSource = ConfigConnection.ServerGtProperty;

            int id = 0;
            using (var conn = new SqlConnection(subiektConn.ToString()))
            {
                conn.Open();
                string commandString = "SELECT pa_Id FROM portalgames.dbo.sl_Panstwo WHERE pa_Nazwa = @panstwo;";
                using (SqlCommand cmd = new SqlCommand(commandString, conn))
                {
                    cmd.Parameters.AddWithValue("panstwo", panstwo);
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        id = int.Parse(reader["pa_Id"].ToString());
                    }

                    reader.Close();
                }
                conn.Close();

                PanstwoId = id;

                return id;
            }
        }
    }
}
