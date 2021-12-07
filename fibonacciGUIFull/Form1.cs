using System;
using System.IO;
using System.Collections.Generic;
using System.Windows.Forms;
using FireSharp.Config;
using FireSharp.Response;
using FireSharp.Interfaces;

namespace fibonacciGUIFull
{

    public partial class Form1 : Form
    {

		/* VARIABLES O PROPIEADES */
		List<int> arregloF = new List<int>();
		int n, nim = 0, xam = 46;

		/* FUNCIONES O MÉTODOS */
		public void Log(string mensaje)
		{
			// Se crea el mensaje del texto a descifrar
			string msj = "Entrada del log : \n" +
				"{" + DateTime.Now.ToLongTimeString() + "}" +
				"{" + DateTime.Now.ToLongDateString() + "}\n" +
				mensaje + "\n- - - - - - - - - - - - -\n";
			File.AppendAllText("logFibonacci.txt", msj);
		}

		public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
			// Revisar que los valores ingresados sean los correctos
			if ((!Int32.TryParse(textBox1.Text, out n)) || ((n <= nim) || (n > xam)))
			{
				string texto = "Error mortal, el número " +
					textBox1.Text +
					" está fuera del intervalo [" +
					nim.ToString() +
					"," +
					xam.ToString() +
					"]";
				// Mandar el mensaje de error al usuario
				MessageBox.Show(texto, "Error en datos", MessageBoxButtons.OK, MessageBoxIcon.Error);
				// Mandar el mensaje al Log
				Log(texto);
				// Una vez que se le haya dado el OK a la ventanita, pues continua con esta parte del programa
				// Limpiar la caja de texto
				textBox1.Text = "";
				// Hacer no visible el botón de guardar
				button2.Visible =false;
				// Colocar el foco a la caja de texto
				textBox1.Focus();
			}
			else
			{
				// Limpiar la caja de texto2 para presentar la serie
				textBox2.Text = "";
				// Calcular la serie con los datos y llenar en la caja de texto con los valores
				for (int k = 0, a = 1, b = 0, c = 0; k < this.n; c = a + b, a = b, b = c, k++)
				{
					arregloF.Add(c);
					textBox2.Text += c.ToString() + ",";
				}

				// Hacer visible el botón de guardar la serie
				button2.Visible = true;
			}
		}
		private void button2_Click(object sender, EventArgs e)
		{
			// Configurar el acceso a la base de datos en tiempo real de FireBase
			IFirebaseConfig ifc = new FirebaseConfig()
			{
				AuthSecret = "1HQd7t7T8G5joitCGQ1qW0Jui3piFOGlvY0aG01o",
				BasePath = "https://adam-ddc87.firebaseio.com/"
			};

			// Se crea un objeto cliente de la base de datos
			IFirebaseClient Client;

			// Intentar realizar la escritura en la base de datos
			try
            {
				// Cargar el cliente con los datos
				Client = new FireSharp.FirebaseClient(ifc);

				// Crear o insertar
				// Crear la clase de donde se guardaran los datos
				SerieFibo sf = new SerieFibo();
				// Cargar los datos en el objeto
				sf.N = textBox1.Text;
				sf.Fecha = "{" + DateTime.Now.ToLongTimeString() + "}" + 
					"{" + DateTime.Now.ToLongDateString() + "}";
				sf.Series = textBox2.Text;

				// Subir a la base
				Client.Set(@"Series/" + sf.N, sf);

				// Mandar el mensaje de que se logró subir a la base
				MessageBox.Show("Datos subidos con éxito", "Guardado", MessageBoxButtons.OK, MessageBoxIcon.Information);

			}
            catch (Exception er)
			{
				string texto = "Error al intentar guardar en la base de datos, no se pudo almacenar";
				MessageBox.Show(texto, "Error al guardar", MessageBoxButtons.OK, MessageBoxIcon.Error);
				// Mandar el mensaje al Log
				Log(texto + "\n" + er.ToString());
			}
			finally
            {
				// Limpiar las cajas de texto
				textBox1.Text = "";
				textBox2.Text = "";
				// Hacer no visible el botón de guardar
				button2.Visible = false;
				// Colocar el foco a la caja de texto
				textBox1.Focus();
			}

		}

		private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Con esto se asegura que solamente se ingresen valores numéricos
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
                e.Handled = true;
            // Cuando se presione la tecla "Enter" pues simplemente realiz aun click
            if (e.KeyChar == (char)Keys.Return)
                button1.PerformClick();
        }
    }

	// Clase para guardar los datos de la serie
	public class SerieFibo
	{
		// Atributos
		public string N { get; set; }
		public string Fecha { get; set; }
		public string Series { get; set; }

	}
}
