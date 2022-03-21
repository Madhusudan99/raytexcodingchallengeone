using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace atmConsoleApp
{
    internal class Program
    {
        private static  SqlConnection conn = new SqlConnection(@"Data Source=EFCYIT-LTR908;Initial Catalog=ATM;Integrated Security=True");

        static void Main(string[] args)
        {
            startApp();
        }

        private static void startApp()
        {


            while (true)
            {
                Console.WriteLine("Enter your choice: \n1. Create account\n2. Credit money to account\n3. Debit money from account\n4. Get balance");
                string userInput = Console.ReadLine();
                bool isValid = false;
                switch (userInput)
                {
                    case "1":
                        Console.WriteLine("Enter your name: ");
                        string name = Console.ReadLine();
                        Console.WriteLine("Enter security pin");
                        int pin = Convert.ToInt32(Console.ReadLine());
                        string query = $"INSERT INTO CUSTOMER VALUES ('{name}', 0, {pin})";
                        SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
                        DataTable dataTable = new DataTable();
                        adapter.Fill(dataTable);



                        Console.WriteLine($"Hi {name} your account has sucessfull been created!!");

                        int accountNumber = getAccountNumberWhere(name);
                        Console.WriteLine($"Your account number is: {accountNumber} use this for making transactions");
                        break;

                    case "2":
                        Console.WriteLine("Enter your account number: ");
                        int userInputAccNo = Convert.ToInt32(Console.ReadLine());
                        isValid = validateUser(userInputAccNo);
                        if (isValid)
                        {
                            creditAmountTo(userInputAccNo);
                            Console.WriteLine("Operation sucessfull!!");
                            printPassbook(userInputAccNo);
                            break;
                        }
                        else
                        {
                            Console.WriteLine("Invalid Credentials");
                            break;
                        }

                        

                    case "3":
                        Console.WriteLine("Enter your account number: ");
                        int userInputAccNo2 = Convert.ToInt32(Console.ReadLine());
                        isValid = validateUser(userInputAccNo2);
                        if (isValid)
                        {
                            debitAmountTo(userInputAccNo2);
                            Console.WriteLine("Operation sucessfull!!");
                            printPassbook(userInputAccNo2);
                            break;
                        }
                        else
                        {
                            Console.WriteLine("Invalid Credentials");
                            break;
                        }

                    case "4":
                        Console.WriteLine("Enter your account number: ");
                        int userInputAccNo3 = Convert.ToInt32(Console.ReadLine());
                        isValid = validateUser(userInputAccNo3);
                        if (isValid)
                        {
                            Console.WriteLine($"Your account balance is: {getBalanceAmt(userInputAccNo3)}");
                            break;
                        }
                        else
                        {
                            Console.WriteLine("Invalid Credentials");
                            break;
                        }
                }
            }
        }

        private static bool validateUser(int userInputAccNo)
        {
            Console.WriteLine("Enter your pin: ");
            int pin = Convert.ToInt32(Console.ReadLine());
            int dbPin = -1;

            string query = $"SELECT * FROM CUSTOMER WHERE id='{userInputAccNo}'";
            SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
            DataTable dataTable = new DataTable();
            adapter.Fill(dataTable);

            foreach (DataRow row in dataTable.Rows)
            {
                dbPin = Convert.ToInt32(row["securityPin"]);
            }

            if (pin == dbPin)
            {
                return true;
            }

            return false;
        }

        private static void printPassbook(int userInputAccNo)
        {
            string query = $"SELECT * FROM PASSBOOK WHERE userID='{userInputAccNo}'";
            SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
            DataTable dataTable = new DataTable();
            adapter.Fill(dataTable);

            foreach (DataRow row in dataTable.Rows)
            {
                Console.WriteLine($"Transaction Id: {row["id"]}\tUser Id: {row["userID"]}\tTransaciton Amount: {row["transactionAmount"]}\tTransaction Type: {row["transactionType"]}");
            }
        }

        private static void debitAmountTo(int userInputAccNo2)
        {
            Console.WriteLine("Enter amount: ");
            int amount = Convert.ToInt32(Console.ReadLine());
            int balanceAmt = getBalanceAmt(userInputAccNo2) - amount;

            string updateBalanceQuery = $"UPDATE CUSTOMER SET availableBalance={balanceAmt} WHERE id={userInputAccNo2}";
            SqlDataAdapter adapter1 = new SqlDataAdapter(updateBalanceQuery, conn);
            DataTable dataTable1 = new DataTable();
            adapter1.Fill(dataTable1);

            string query = $"INSERT INTO PASSBOOK VALUES ({userInputAccNo2}, -{amount}, 'DEBIT')";
            SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
            DataTable dataTable = new DataTable();
            adapter.Fill(dataTable);
        }

        private static void creditAmountTo(int userInputAccNo)
        {
            Console.WriteLine("Enter amount: ");
            int amount = Convert.ToInt32(Console.ReadLine());
            int balanceAmt = getBalanceAmt(userInputAccNo) + amount;

            string updateBalanceQuery = $"UPDATE CUSTOMER SET availableBalance={balanceAmt} WHERE id={userInputAccNo}";

            SqlDataAdapter adapter1 = new SqlDataAdapter(updateBalanceQuery, conn);
            DataTable dataTable1 = new DataTable();
            adapter1.Fill(dataTable1);
            string query = $"INSERT INTO PASSBOOK VALUES ({userInputAccNo}, {amount}, 'CREDIT')";
            SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
            DataTable dataTable = new DataTable();
            adapter.Fill(dataTable);

        }

        private static int getBalanceAmt(int userInputAccNo)
        {

            int balanceAmt = -1;

            string query = $"SELECT * FROM CUSTOMER WHERE id='{userInputAccNo}'";
            SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
            DataTable dataTable = new DataTable();
            adapter.Fill(dataTable);

            foreach (DataRow row in dataTable.Rows)
            {
                balanceAmt = Convert.ToInt32(row["availableBalance"]);
            }

            return balanceAmt;
        }

        private static int getAccountNumberWhere(string name)
        {

            int accountNumber = -1;

            string query = $"SELECT * FROM CUSTOMER WHERE name='{name}'";
            SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
            DataTable dataTable = new DataTable();
            adapter.Fill(dataTable);

            foreach (DataRow row in dataTable.Rows)
            {
                accountNumber = Convert.ToInt32(row["id"]);
            }

            return accountNumber;
        }
    }
}
