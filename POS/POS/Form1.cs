using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Dapper;
using ServiceStack;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace POS
{
    public partial class Form1 : Form
    {
        List<Product> temp = new List<Product>();
        List<Product> checkout = new List<Product>();
        public Form1()
        {
            InitializeComponent();
        }

        public double Cost_of_Items()
        {
            Double sum = 0;
            int i = 0;
            for (i = 0; i < (cartDataGrid.Rows.Count); i++)
            {
                sum = sum + Convert.ToDouble(cartDataGrid.Rows[i].Cells[2].Value);
            }
            return sum;
        }

        private void AddCost()
        {
            Double tax, q;
            tax = 3.9;
            if(cartDataGrid.Rows.Count > 0)
            {
                taxDisplayBox.Text = String.Format("{0:c2}", (((Cost_of_Items() * tax) / 100)));
                subtotalDisplayBox.Text = String.Format("{0:c2}", (Cost_of_Items()));
                q = ((Cost_of_Items() * tax) / 100);
                totalDisplayBox.Text =String.Format("{0:c2}", ((Cost_of_Items() + q)));
                cartBarcodeDisplayBox.Text = Convert.ToString(q + Cost_of_Items());
            }
        }

        private void Change()
        {
            Double tax, q, c;
            tax = 3.9;
            if (cartDataGrid.Rows.Count > 0)
            {
                q = ((Cost_of_Items() * tax) / 100) + Cost_of_Items();
                c = Convert.ToInt32(costDisplayBox.Text);
                changeDisplayBox.Text = String.Format("{0:c2}", c - q);
            }
        }

                Bitmap bitmap;
        private void printReceiptOption_Click(object sender, EventArgs e)
        {
            
            cartDataGrid.Rows.Add("Subtotal", " ", subtotalDisplayBox.Text);
            cartDataGrid.Rows.Add("Tax", " ", taxDisplayBox.Text);
            cartDataGrid.Rows.Add("Total", " ", totalDisplayBox.Text);
            cartDataGrid.Rows.Add("Amount Paid", " ", "£" + costDisplayBox.Text);
            cartDataGrid.Rows.Add("Change", " ", changeDisplayBox.Text);
            
            
            try
            {
                int height = cartDataGrid.Height;
                cartDataGrid.Height = cartDataGrid.RowCount * cartDataGrid.RowTemplate.Height * 2;
                bitmap = new Bitmap(cartDataGrid.Width, cartDataGrid.Height);
                cartDataGrid.DrawToBitmap(bitmap, new Rectangle(0, 0, cartDataGrid.Width, cartDataGrid.Height));
                foreach (DataGridViewRow row in cartDataGrid.Rows)
                {
                   if (row.Cells[0].Value.ToString().Equals("Subtotal") && row.Cells[1].Value != null)
                    {
                        cartDataGrid.Rows.Remove(row);
                        break;
                    }

                }

                foreach (DataGridViewRow row in cartDataGrid.Rows)
                {
                    if (row.Cells[0].Value.ToString().Equals("Tax") && row.Cells[1].Value != null)
                    {
                        cartDataGrid.Rows.Remove(row);
                        break;
                    }

                }

                foreach (DataGridViewRow row in cartDataGrid.Rows)
                {

                    if (row.Cells[0].Value.ToString().Equals("Total") && row.Cells[1].Value != null)
                    {
                        cartDataGrid.Rows.Remove(row);
                        break;
                    }
                }

                foreach (DataGridViewRow row in cartDataGrid.Rows)
                {

                    if (row.Cells[0].Value.ToString().Equals("Amount Paid") && row.Cells[1].Value != null)
                    {
                        cartDataGrid.Rows.Remove(row);
                        break;
                    }
                }

                foreach (DataGridViewRow row in cartDataGrid.Rows)
                {

                    if (row.Cells[0].Value.ToString().Equals("Change") && row.Cells[1].Value != null)
                    {
                        cartDataGrid.Rows.Remove(row);
                        break;
                    }
                }

                printPreviewDialog1.PrintPreviewControl.Zoom = 1;
                printPreviewDialog1.ShowDialog();
                cartDataGrid.Height = height;
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
           
            
            
        }
        private void resetCartOption_Click(object sender, EventArgs e)
        {
            changeDisplayBox.Text = "0";
            cartBarcodeDisplayBox.Text = "";
            costDisplayBox.Text = "0";
            subtotalDisplayBox.Text = "00";
            taxDisplayBox.Text = "";
            totalDisplayBox.Text = "";
            typeOfPaymentMenu.Text = "";
            cartDataGrid.Rows.Clear();
            cartDataGrid.Refresh();
            checkout.Clear();
        }

        private void printDocument1_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            try
            {
                e.Graphics.DrawImage(bitmap, 0, 0);
            }

            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            typeOfPaymentMenu.Items.Add("Cash");
            typeOfPaymentMenu.Items.Add("Visa Card");
            typeOfPaymentMenu.Items.Add("Master Card");

        }

        private void NumbersOnly(object sender, EventArgs e)
        {
            System.Windows.Forms.Button b = (System.Windows.Forms.Button)sender;
            if (costDisplayBox.Text == "0")
            {
                costDisplayBox.Text = "";
                costDisplayBox.Text = b.Text;
            }
            else if (b.Text == ".")
            {
                if (! costDisplayBox.Text.Contains("."))
                {
                    costDisplayBox.Text = costDisplayBox.Text + b.Text;
                }
            }
            else
                costDisplayBox.Text = costDisplayBox.Text + b.Text;
        }

        private void keypadClearContent_Click(object sender, EventArgs e)
        {
            costDisplayBox.Text = "0";
        }

        private void payOption_Click(object sender, EventArgs e)
        {
            if (typeOfPaymentMenu.Text == "Cash")
            {
                Change();
            }
            else
            {
                changeDisplayBox.Text = "";
                costDisplayBox.Text = "0";
            }
        }

        private void removeItemOption_Click(object sender, EventArgs e)
        {
            foreach(DataGridViewRow row in this.cartDataGrid.SelectedRows)
            {
                cartDataGrid.Rows.Remove(row);
                foreach(Product product in checkout)
                {
                    if (row.Cells[0].Value.ToString().Equals(product.Name) && row.Cells[1].Value != null)
                    {
                        checkout.Remove(product);
                        break;
                    }
                }
            }
            AddCost();
            if (typeOfPaymentMenu.Text == "Cash")
            {
                Change();
            }
            else
            {
                changeDisplayBox.Text = "";
                costDisplayBox.Text = "0";
            }
        }

        private void cappuccino_Click(object sender, EventArgs e)
        {
            int id = 2;
            var sql = "select * from Products" +
               " where ID = " + id;
            using (SqlConnection connection = new SqlConnection("Data Source=\"51.19.167.213, 1433\";Persist Security Info=True;User ID=sa;Password="))
            {
                connection.Open();

                temp = connection.Query<Product>(sql).ToList();
                foreach (Product p in temp)
                {
                    p.stock = 1;
                }
                connection.Close();
            }


            foreach (Product product in temp)
            {
                if (checkout.Count != 0)
                {
                    if (checkout.Any(prod => prod.id == product.id))
                    {
                        cartDataGrid.Rows.Clear();
                        foreach (Product p in checkout)
                        {
                            if (id == p.id)
                            {
                                p.stock++;
                                product.stock = p.stock;
                            }
                            cartDataGrid.Rows.Add(p.Name, p.stock, p.price * p.stock);
                        }

                    }
                    else
                    {
                        checkout.Add(product);
                        cartDataGrid.Rows.Add(product.Name, product.stock, product.price);
                    }
                }
                else
                {
                    checkout.Add(product);
                    cartDataGrid.Rows.Add(product.Name, product.stock, product.price);
                }

            }
            //Double CostofItem = 4.00;
            //foreach (DataGridViewRow row in this.cartDataGrid.SelectedRows)
            //{
            //    if ((bool)(row.Cells[0].Value = "Cappuccino"))
            //    {
            //        row.Cells[1].Value = Double.Parse((string)row.Cells[1].Value + 1);
            //        row.Cells[2].Value = Double.Parse((string)row.Cells[1].Value) * CostofItem;
            //    }
            //}
            //cartDataGrid.Rows.Add("Cappuccino", "1", CostofItem);
            AddCost();
        }

        private void latte_Click(object sender, EventArgs e)
        {
            int id = 1;
            var sql = "select * from Products" +
               " where ID = " + id;
            using (SqlConnection connection = new SqlConnection("Data Source=\"51.19.167.213, 1433\";Persist Security Info=True;User ID=sa;Password="))
            {
                connection.Open();

                temp = connection.Query<Product>(sql).ToList();
                foreach (Product p in temp)
                {
                    p.stock = 1;
                }
                connection.Close();
            }


            foreach (Product product in temp)
            {
                if (checkout.Count != 0)
                {
                    if (checkout.Any(prod => prod.id == product.id))
                    {
                        cartDataGrid.Rows.Clear();
                        foreach (Product p in checkout)
                        {
                            if (id == p.id)
                            {
                                p.stock++;
                                product.stock = p.stock;
                            }
                            cartDataGrid.Rows.Add(p.Name, p.stock, p.price * p.stock);
                        }

                    }
                    else
                    {
                        checkout.Add(product);
                        cartDataGrid.Rows.Add(product.Name, product.stock, product.price);
                    }
                }
                else
                {
                    checkout.Add(product);
                    cartDataGrid.Rows.Add(product.Name, product.stock, product.price);
                }

            }
            //Double CostofItem = 4.00;
            //foreach (DataGridViewRow row in this.cartDataGrid.SelectedRows)
            //{
            //    if ((bool)(row.Cells[0].Value = "Latte"))
            //    {
            //        row.Cells[1].Value = Double.Parse((string)row.Cells[1].Value + 1);
            //        row.Cells[2].Value = Double.Parse((string)row.Cells[1].Value) * CostofItem;
            //    }
            //}
            //cartDataGrid.Rows.Add("Latte", "1", CostofItem);
            AddCost();
        }

        private void americanoBlack_Click(object sender, EventArgs e)
        {
            int id = 3;
            var sql = "select * from Products" +
               " where ID = " + id;
            using (SqlConnection connection = new SqlConnection("Data Source=\"51.19.167.213, 1433\";Persist Security Info=True;User ID=sa;Password="))
            {
                connection.Open();

                temp = connection.Query<Product>(sql).ToList();
                foreach (Product p in temp)
                {
                    p.stock = 1;
                }
                connection.Close();
            }


            foreach (Product product in temp)
            {
                if (checkout.Count != 0)
                {
                    if (checkout.Any(prod => prod.id == product.id))
                    {
                        cartDataGrid.Rows.Clear();
                        foreach (Product p in checkout)
                        {
                            if (id == p.id)
                            {
                                p.stock++;
                                product.stock = p.stock;
                            }
                            cartDataGrid.Rows.Add(p.Name, p.stock, p.price * p.stock);
                        }

                    }
                    else
                    {
                        checkout.Add(product);
                        cartDataGrid.Rows.Add(product.Name, product.stock, product.price);
                    }
                }
                else
                {
                    checkout.Add(product);
                    cartDataGrid.Rows.Add(product.Name, product.stock, product.price);
                }

            }
            //Double CostofItem = 3.50;
            //foreach (DataGridViewRow row in this.cartDataGrid.SelectedRows)
            //{
            //    if ((bool)(row.Cells[0].Value = "Americano Black"))
            //    {
            //        row.Cells[1].Value = Double.Parse((string)row.Cells[1].Value + 1);
            //        row.Cells[2].Value = Double.Parse((string)row.Cells[1].Value) * CostofItem;
            //    }
            //}
            //cartDataGrid.Rows.Add("Americano Black", "1", CostofItem);
            AddCost();
        }

        private void americanoWhite_Click(object sender, EventArgs e)
        {
            int id = 4;
            var sql = "select * from Products" +
               " where ID = " + id;
            using (SqlConnection connection = new SqlConnection("Data Source=\"51.19.167.213, 1433\";Persist Security Info=True;User ID=sa;Password="))
            {
                connection.Open();

                temp = connection.Query<Product>(sql).ToList();
                foreach (Product p in temp)
                {
                    p.stock = 1;
                }
                connection.Close();
            }


            foreach (Product product in temp)
            {
                if (checkout.Count != 0)
                {
                    if (checkout.Any(prod => prod.id == product.id))
                    {
                        cartDataGrid.Rows.Clear();
                        foreach (Product p in checkout)
                        {
                            if (id == p.id)
                            {
                                p.stock++;
                                product.stock = p.stock;
                            }
                            cartDataGrid.Rows.Add(p.Name, p.stock, p.price * p.stock);
                        }

                    }
                    else
                    {
                        checkout.Add(product);
                        cartDataGrid.Rows.Add(product.Name, product.stock, product.price);
                    }
                }
                else
                {
                    checkout.Add(product);
                    cartDataGrid.Rows.Add(product.Name, product.stock, product.price);
                }

            }
            //Double CostofItem = 3.50;
            //foreach (DataGridViewRow row in this.cartDataGrid.SelectedRows)
            //{
            //    if ((bool)(row.Cells[0].Value = "Americano White"))
            //    {
            //        row.Cells[1].Value = Double.Parse((string)row.Cells[1].Value + 1);
            //        row.Cells[2].Value = Double.Parse((string)row.Cells[1].Value) * CostofItem;
            //    }
            //}
            //cartDataGrid.Rows.Add("Americano White", "1", CostofItem);
            AddCost();
        }

        private void flatWhite_Click(object sender, EventArgs e)
        {
            int id = 5;
            var sql = "select * from Products" +
               " where ID = " + id;
            using (SqlConnection connection = new SqlConnection("Data Source=\"51.19.167.213, 1433\";Persist Security Info=True;User ID=sa;Password="))
            {
                connection.Open();

                temp = connection.Query<Product>(sql).ToList();
                foreach (Product p in temp)
                {
                    p.stock = 1;
                }
                connection.Close();
            }


            foreach (Product product in temp)
            {
                if (checkout.Count != 0)
                {
                    if (checkout.Any(prod => prod.id == product.id))
                    {
                        cartDataGrid.Rows.Clear();
                        foreach (Product p in checkout)
                        {
                            if (id == p.id)
                            {
                                p.stock++;
                                product.stock = p.stock;
                            }
                            cartDataGrid.Rows.Add(p.Name, p.stock, p.price * p.stock);
                        }

                    }
                    else
                    {
                        checkout.Add(product);
                        cartDataGrid.Rows.Add(product.Name, product.stock, product.price);
                    }
                }
                else
                {
                    checkout.Add(product);
                    cartDataGrid.Rows.Add(product.Name, product.stock, product.price);
                }

            }
            //Double CostofItem = 3.80;
            //foreach (DataGridViewRow row in this.cartDataGrid.SelectedRows)
            //{
            //    if ((bool)(row.Cells[0].Value = "Flat White"))
            //    {
            //        row.Cells[1].Value = Double.Parse((string)row.Cells[1].Value + 1);
            //        row.Cells[2].Value = Double.Parse((string)row.Cells[1].Value) * CostofItem;
            //    }
            //}
            //cartDataGrid.Rows.Add("Flat White", "1", CostofItem);
            AddCost();
        }

        private void flatBlack_Click(object sender, EventArgs e)
        {
            int id = 6;
            var sql = "select * from Products" +
               " where ID = " + id;
            using (SqlConnection connection = new SqlConnection("Data Source=\"51.19.167.213, 1433\";Persist Security Info=True;User ID=sa;Password="))
            {
                connection.Open();

                temp = connection.Query<Product>(sql).ToList();
                foreach (Product p in temp)
                {
                    p.stock = 1;
                }
                connection.Close();
            }


            foreach (Product product in temp)
            {
                if (checkout.Count != 0)
                {
                    if (checkout.Any(prod => prod.id == product.id))
                    {
                        cartDataGrid.Rows.Clear();
                        foreach (Product p in checkout)
                        {
                            if (id == p.id)
                            {
                                p.stock++;
                                product.stock = p.stock;
                            }
                            cartDataGrid.Rows.Add(p.Name, p.stock, p.price * p.stock);
                        }

                    }
                    else
                    {
                        checkout.Add(product);
                        cartDataGrid.Rows.Add(product.Name, product.stock, product.price);
                    }
                }
                else
                {
                    checkout.Add(product);
                    cartDataGrid.Rows.Add(product.Name, product.stock, product.price);
                }

            }
            //Double CostofItem = 3.35;
            //foreach (DataGridViewRow row in this.cartDataGrid.SelectedRows)
            //{
            //    if ((bool)(row.Cells[0].Value = "Flat Black"))
            //    {
            //        row.Cells[1].Value = Double.Parse((string)row.Cells[1].Value + 1);
            //        row.Cells[2].Value = Double.Parse((string)row.Cells[1].Value) * CostofItem;
            //    }
            //}
            //cartDataGrid.Rows.Add("Flat Black", "1", CostofItem);
            AddCost();
        }

        private void cortado_Click(object sender, EventArgs e)
        {
            int id = 7;
            var sql = "select * from Products" +
               " where ID = " + id;
            using (SqlConnection connection = new SqlConnection("Data Source=\"51.19.167.213, 1433\";Persist Security Info=True;User ID=sa;Password="))
            {
                connection.Open();

                temp = connection.Query<Product>(sql).ToList();
                foreach (Product p in temp)
                {
                    p.stock = 1;
                }
                connection.Close();
            }


            foreach (Product product in temp)
            {
                if (checkout.Count != 0)
                {
                    if (checkout.Any(prod => prod.id == product.id))
                    {
                        cartDataGrid.Rows.Clear();
                        foreach (Product p in checkout)
                        {
                            if (id == p.id)
                            {
                                p.stock++;
                                product.stock = p.stock;
                            }
                            cartDataGrid.Rows.Add(p.Name, p.stock, p.price * p.stock);
                        }

                    }
                    else
                    {
                        checkout.Add(product);
                        cartDataGrid.Rows.Add(product.Name, product.stock, product.price);
                    }
                }
                else
                {
                    checkout.Add(product);
                    cartDataGrid.Rows.Add(product.Name, product.stock, product.price);
                }

            }
            //Double CostofItem = 3.40;
            //foreach (DataGridViewRow row in this.cartDataGrid.SelectedRows)
            //{
            //    if ((bool)(row.Cells[0].Value = "Cortado"))
            //    {
            //        row.Cells[1].Value = Double.Parse((string)row.Cells[1].Value + 1);
            //        row.Cells[2].Value = Double.Parse((string)row.Cells[1].Value) * CostofItem;
            //    }
            //}
            //cartDataGrid.Rows.Add("Cortado", "1", CostofItem);
            AddCost();
        }

        private void caramelCortado_Click(object sender, EventArgs e)
        {
            int id = 8;
            var sql = "select * from Products" +
               " where ID = " + id;
            using (SqlConnection connection = new SqlConnection("Data Source=\"51.19.167.213, 1433\";Persist Security Info=True;User ID=sa;Password="))
            {
                connection.Open();

                temp = connection.Query<Product>(sql).ToList();
                foreach (Product p in temp)
                {
                    p.stock = 1;
                }
                connection.Close();
            }


            foreach (Product product in temp)
            {
                if (checkout.Count != 0)
                {
                    if (checkout.Any(prod => prod.id == product.id))
                    {
                        cartDataGrid.Rows.Clear();
                        foreach (Product p in checkout)
                        {
                            if (id == p.id)
                            {
                                p.stock++;
                                product.stock = p.stock;
                            }
                            cartDataGrid.Rows.Add(p.Name, p.stock, p.price * p.stock);
                        }

                    }
                    else
                    {
                        checkout.Add(product);
                        cartDataGrid.Rows.Add(product.Name, product.stock, product.price);
                    }
                }
                else
                {
                    checkout.Add(product);
                    cartDataGrid.Rows.Add(product.Name, product.stock, product.price);
                }

            }
            //Double CostofItem = 3.50;
            //foreach (DataGridViewRow row in this.cartDataGrid.SelectedRows)
            //{
            //    if ((bool)(row.Cells[0].Value = "Caramel Cortado"))
            //    {
            //        row.Cells[1].Value = Double.Parse((string)row.Cells[1].Value + 1);
            //        row.Cells[2].Value = Double.Parse((string)row.Cells[1].Value) * CostofItem;
            //    }
            //}
            //cartDataGrid.Rows.Add("Caramel Cortado", "1", CostofItem);
            AddCost();
        }

        private void mocha_Click(object sender, EventArgs e)
        {
            int id = 9;
            var sql = "select * from Products" +
               " where ID = " + id;
            using (SqlConnection connection = new SqlConnection("Data Source=\"51.19.167.213, 1433\";Persist Security Info=True;User ID=sa;Password="))
            {
                connection.Open();

                temp = connection.Query<Product>(sql).ToList();
                foreach (Product p in temp)
                {
                    p.stock = 1;
                }
                connection.Close();
            }


            foreach (Product product in temp)
            {
                if (checkout.Count != 0)
                {
                    if (checkout.Any(prod => prod.id == product.id))
                    {
                        cartDataGrid.Rows.Clear();
                        foreach (Product p in checkout)
                        {
                            if (id == p.id)
                            {
                                p.stock++;
                                product.stock = p.stock;
                            }
                            cartDataGrid.Rows.Add(p.Name, p.stock, p.price * p.stock);
                        }

                    }
                    else
                    {
                        checkout.Add(product);
                        cartDataGrid.Rows.Add(product.Name, product.stock, product.price);
                    }
                }
                else
                {
                    checkout.Add(product);
                    cartDataGrid.Rows.Add(product.Name, product.stock, product.price);
                }

            }
            //Double CostofItem = 4.15;
            //foreach (DataGridViewRow row in this.cartDataGrid.SelectedRows)
            //{
            //    if ((bool)(row.Cells[0].Value = "Mocha"))
            //    {
            //        row.Cells[1].Value = Double.Parse((string)row.Cells[1].Value + 1);
            //        row.Cells[2].Value = Double.Parse((string)row.Cells[1].Value) * CostofItem;
            //    }
            //}
            //cartDataGrid.Rows.Add("Mocha", "1", CostofItem);
            AddCost();
        }

        private void hotChocolate_Click(object sender, EventArgs e)
        {
            int id = 10;
            var sql = "select * from Products" +
               " where ID = " + id;
            using (SqlConnection connection = new SqlConnection("Data Source=\"51.19.167.213, 1433\";Persist Security Info=True;User ID=sa;Password="))
            {
                connection.Open();

                temp = connection.Query<Product>(sql).ToList();
                foreach (Product p in temp)
                {
                    p.stock = 1;
                }
                connection.Close();
            }


            foreach (Product product in temp)
            {
                if (checkout.Count != 0)
                {
                    if (checkout.Any(prod => prod.id == product.id))
                    {
                        cartDataGrid.Rows.Clear();
                        foreach (Product p in checkout)
                        {
                            if (id == p.id)
                            {
                                p.stock++;
                                product.stock = p.stock;
                            }
                            cartDataGrid.Rows.Add(p.Name, p.stock, p.price * p.stock);
                        }

                    }
                    else
                    {
                        checkout.Add(product);
                        cartDataGrid.Rows.Add(product.Name, product.stock, product.price);
                    }
                }
                else
                {
                    checkout.Add(product);
                    cartDataGrid.Rows.Add(product.Name, product.stock, product.price);
                }

            }
            //Double CostofItem = 4.05;
            //foreach (DataGridViewRow row in this.cartDataGrid.SelectedRows)
            //{
            //    if ((bool)(row.Cells[0].Value = "Hot Chocolate"))
            //    {
            //        row.Cells[1].Value = Double.Parse((string)row.Cells[1].Value + 1);
            //        row.Cells[2].Value = Double.Parse((string)row.Cells[1].Value) * CostofItem;
            //    }
            //}
            //cartDataGrid.Rows.Add("Hot Chocolate", "1", CostofItem);
            AddCost();
        }

        private void roloChocolateCaramelHotChocolate_Click(object sender, EventArgs e)
        {
            int id = 11;
            var sql = "select * from Products" +
               " where ID = " + id;
            using (SqlConnection connection = new SqlConnection("Data Source=\"51.19.167.213, 1433\";Persist Security Info=True;User ID=sa;Password="))
            {
                connection.Open();

                temp = connection.Query<Product>(sql).ToList();
                foreach (Product p in temp)
                {
                    p.stock = 1;
                }
                connection.Close();
            }


            foreach (Product product in temp)
            {
                if (checkout.Count != 0)
                {
                    if (checkout.Any(prod => prod.id == product.id))
                    {
                        cartDataGrid.Rows.Clear();
                        foreach (Product p in checkout)
                        {
                            if (id == p.id)
                            {
                                p.stock++;
                                product.stock = p.stock;
                            }
                            cartDataGrid.Rows.Add(p.Name, p.stock, p.price * p.stock);
                        }

                    }
                    else
                    {
                        checkout.Add(product);
                        cartDataGrid.Rows.Add(product.Name, product.stock, product.price);
                    }
                }
                else
                {
                    checkout.Add(product);
                    cartDataGrid.Rows.Add(product.Name, product.stock, product.price);
                }

            }
            //Double CostofItem = 4.85;
            //foreach (DataGridViewRow row in this.cartDataGrid.SelectedRows)
            //{
            //    if ((bool)(row.Cells[0].Value = "Rolo Chocolate Caramel Hot Chocolate"))
            //    {
            //        row.Cells[1].Value = Double.Parse((string)row.Cells[1].Value + 1);
            //        row.Cells[2].Value = Double.Parse((string)row.Cells[1].Value) * CostofItem;
            //    }
            //}
            //cartDataGrid.Rows.Add("Rolo Chocolate Caramel Hot Chocolate", "1", CostofItem);
            AddCost();
        }

        private void whiteHotChocolate_Click(object sender, EventArgs e)
        {
            int id = 12;
            var sql = "select * from Products" +
               " where ID = " + id;
            using (SqlConnection connection = new SqlConnection("Data Source=\"51.19.167.213, 1433\";Persist Security Info=True;User ID=sa;Password="))
            {
                connection.Open();

                temp = connection.Query<Product>(sql).ToList();
                foreach (Product p in temp)
                {
                    p.stock = 1;
                }
                connection.Close();
            }


            foreach (Product product in temp)
            {
                if (checkout.Count != 0)
                {
                    if (checkout.Any(prod => prod.id == product.id))
                    {
                        cartDataGrid.Rows.Clear();
                        foreach (Product p in checkout)
                        {
                            if (id == p.id)
                            {
                                p.stock++;
                                product.stock = p.stock;
                            }
                            cartDataGrid.Rows.Add(p.Name, p.stock, p.price * p.stock);
                        }

                    }
                    else
                    {
                        checkout.Add(product);
                        cartDataGrid.Rows.Add(product.Name, product.stock, product.price);
                    }
                }
                else
                {
                    checkout.Add(product);
                    cartDataGrid.Rows.Add(product.Name, product.stock, product.price);
                }

            }
            //Double CostofItem = 4.05;
            //foreach (DataGridViewRow row in this.cartDataGrid.SelectedRows)
            //{
            //    if ((bool)(row.Cells[0].Value = "White Hot Chocolate"))
            //    {
            //        row.Cells[1].Value = Double.Parse((string)row.Cells[1].Value + 1);
            //        row.Cells[2].Value = Double.Parse((string)row.Cells[1].Value) * CostofItem;
            //    }
            //}
            //cartDataGrid.Rows.Add("White Hot Chocolate", "1", CostofItem);
            AddCost();
        }

        private void chaiLatte_Click(object sender, EventArgs e)
        {
            int id = 13;
            var sql = "select * from Products" +
               " where ID = " + id;
            using (SqlConnection connection = new SqlConnection("Data Source=\"51.19.167.213, 1433\";Persist Security Info=True;User ID=sa;Password="))
            {
                connection.Open();

                temp = connection.Query<Product>(sql).ToList();
                foreach (Product p in temp)
                {
                    p.stock = 1;
                }
                connection.Close();
            }


            foreach (Product product in temp)
            {
                if (checkout.Count != 0)
                {
                    if (checkout.Any(prod => prod.id == product.id))
                    {
                        cartDataGrid.Rows.Clear();
                        foreach (Product p in checkout)
                        {
                            if (id == p.id)
                            {
                                p.stock++;
                                product.stock = p.stock;
                            }
                            cartDataGrid.Rows.Add(p.Name, p.stock, p.price * p.stock);
                        }

                    }
                    else
                    {
                        checkout.Add(product);
                        cartDataGrid.Rows.Add(product.Name, product.stock, product.price);
                    }
                }
                else
                {
                    checkout.Add(product);
                    cartDataGrid.Rows.Add(product.Name, product.stock, product.price);
                }

            }
            //Double CostofItem = 4.25;
            //foreach (DataGridViewRow row in this.cartDataGrid.SelectedRows)
            //{
            //    if ((bool)(row.Cells[0].Value = "Chai Latte"))
            //    {
            //        row.Cells[1].Value = Double.Parse((string)row.Cells[1].Value + 1);
            //        row.Cells[2].Value = Double.Parse((string)row.Cells[1].Value) * CostofItem;
            //    }
            //}
            //cartDataGrid.Rows.Add("Chai Latte", "1", CostofItem);
            AddCost();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Form2 stock = new Form2();
            stock.Show();
        }

        private void searchItemOption_Click(object sender, EventArgs e)
        {
            string searchValue = searchItemBox.Text;

            cartDataGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            try
            {
                foreach (DataGridViewRow row in cartDataGrid.Rows)
                {
                    if (row.Cells[0].Value.ToString().Equals(searchValue) && row.Cells[1].Value != null)
                    {
                        row.Selected = true;
                        break;
                    }
                }
            }
            catch (Exception exc)
            {
                MessageBox.Show("The item is not in the list");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            cartDataGrid.Rows.Clear();

            var sql = "select * from Products";
            using (SqlConnection connection = new SqlConnection("Data Source=\"51.19.167.213, 1433\";Persist Security Info=True;User ID=sa;Password="))
            {
                connection.Open();

                temp = connection.Query<Product>(sql).ToList();
                connection.Close();
            }
            
            foreach (Product row in checkout)
            {
                string query = "update Product set Stock = Stock - " + row.stock + " where Product_Name = '" + row.Name + "';";
                using (SqlConnection connection = new SqlConnection("Data Source=\"51.19.167.213, 1433\";Persist Security Info=True;User ID=sa;Password="))
                {
                    connection.Open();
                    using (SqlCommand stockupdate = new SqlCommand(query, connection))
                    {
                        stockupdate.Parameters.AddWithValue("Stock", row.stock);
                        int i = stockupdate.ExecuteNonQuery();
                        if (i > 0)
                        {
                            MessageBox.Show("Successful");
                        }
                        
                    }
                }
                    
            }
            foreach(Product row in checkout) 
            {
                using(System.IO.StreamWriter file = new System.IO.StreamWriter(@"C:\Users\kk195\Downloads\POS - Themathics\POS\Data.csv", true))
                {
                    file.WriteLine(row.Name + "," + row.stock.ToString() + "," + row.price.ToString());
                }
            }
            checkout.Clear();
        }

        private void panel3_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            cartDataGrid.Rows.Clear();

            var sql = "select * from Products";
            using (SqlConnection connection = new SqlConnection("Data Source=\"51.19.167.213, 1433\";Persist Security Info=True;User ID=sa;Password="))
            {
                connection.Open();

                temp = connection.Query<Product>(sql).ToList();
                connection.Close();
            }

            foreach (Product row in checkout)
            {
                string query = "update Products set Stock = Stock - " + row.stock + " where Product_Name = '" + row.Name + "';";
                using (SqlConnection connection = new SqlConnection("Data Source=\"51.19.167.213, 1433\";Persist Security Info=True;User ID=sa;Password="))
                {
                    connection.Open();
                    using (SqlCommand stockupdate = new SqlCommand(query, connection))
                    {
                        stockupdate.Parameters.AddWithValue("Stock", row.stock);
                        int i = stockupdate.ExecuteNonQuery();
                        if (i > 0)
                        {
                            MessageBox.Show("Successful");
                        }

                    }
                }

            }
            foreach (Product row in checkout)
            {
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"Data.csv", true))
                {
                    file.WriteLine(DateTime.Now + "," + row.Name + "," + row.stock.ToString() + "," + row.price.ToString());
                }
            }
            checkout.Clear();
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            Form2 stock = new Form2();
            stock.Show();
        }

        private void searchItemOption_Click_1(object sender, EventArgs e)
        {
            string searchValue = searchItemBox.Text;

            cartDataGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            try
            {
                foreach (DataGridViewRow row in cartDataGrid.Rows)
                {
                    if (row.Cells[0].Value.ToString().Equals(searchValue) && row.Cells[1].Value != null)
                    {
                        row.Selected = true;
                        break;
                    }
                }
            }
            catch (Exception exc)
            {
                MessageBox.Show("The item is not in the list");
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Form3 sales = new Form3();
            sales.Show();
            
        }

        private void costDisplayBox_Click(object sender, EventArgs e)
        {

        }

        private void typeOfPaymentMenu_SelectedIndexChanged(object sender, EventArgs e)
        {

            if (typeOfPaymentMenu.Text == "Cash")
            {
                
            }
            else
            {
                changeDisplayBox.Text = "";
                costDisplayBox.Text = totalDisplayBox.Text;
            }
        }
    }
}
