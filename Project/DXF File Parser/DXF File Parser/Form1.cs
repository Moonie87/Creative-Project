using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DXF_File_Parser
{
    public partial class Parser : Form
    {
        #region Variables

        
        public String inputFileTxt;      

        // creates a open dialog
        private OpenFileDialog openFileDialog1;      
        // creates four double variables
        private double XMax, XMin;
        private double YMax, YMin;
        // creates the fileinfo
        private FileInfo theSourceFile;

#endregion

        #region Parser initialization and main

        public Parser()
        {
            InitializeComponent();
        }

       

        static void main()
        {
            Application.Run(new Parser());
        }

        #endregion

        #region Deals with button presses

        private void toolStripMenuItem3_Click(object sender, EventArgs e)
        {
            inputFileTxt = "";

            openFileDialog1.InitialDirectory = "c:\\";		//sets the initial directory of the openfile dialog

            openFileDialog1.Filter = "dxf files (*.dxf)|*.dxf|All files (*.*)|*.*";	//filters the visible files...

            openFileDialog1.FilterIndex = 1;


            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)		//open file dialog is shown here...if "cancel" button is clicked then nothing will be done...
            {
                inputFileTxt = openFileDialog1.FileName;	//filename is taken (file path is also included to this name example: c:\windows\system\blabla.dxf

                int ino = inputFileTxt.LastIndexOf("\\");	//index no of the last "\" (that is before the filename) is found here


               // newCanvas = new Canvas();		//a new canvas is created...			


                if (inputFileTxt.Length > 0)
                {
                    ReadFromFile(inputFileTxt);		//the filename is sent to the method for data extraction and interpretation...
                }

            }

            openFileDialog1.Dispose();
        }

        private void toolStripMenuItem4_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            AboutBox1 a = new AboutBox1();
            a.Show();
        }

        #endregion

        #region deals with reading and writing files

        public void ReadFromFile(string textFile)
        {
            

            // lines are initilized 
            string line1 = "0";									
            string line2 = "0";

            //the sourcefile is set
            theSourceFile = new FileInfo(textFile);		

            // reader is initilized to null
            StreamReader reader = null;		

            try
            {
                //sets the reader
                reader = theSourceFile.OpenText();			
            }
            // used for catching an error
            catch (FileNotFoundException e)
            {
                MessageBox.Show(e.FileName.ToString() + " cannot be found");
            }
            catch
            {
                MessageBox.Show("An error occurred while opening the DXF file");
                return;
            }
            do
            {

                if (line1 == "0" && line2 == "LINE")
                    LineModule(reader);

                //function is called for iterating through the text file and assigning values to line1 and line2
                GetLineCouple(reader, out line1, out line2);		

            }
            while (line2 != "EOF");

            //reader is cleared
            reader.DiscardBufferedData();							
            theSourceFile = null;

            //reader is closed
            reader.Close();											
        }


        //function is used to iterate through the text file and assign values to line1 and line2
        private void GetLineCouple(StreamReader theReader, out string line1, out string line2)		
        {
            // creates a CultureInfo and decimal seperator.
            System.Globalization.CultureInfo ci = System.Threading.Thread.CurrentThread.CurrentCulture;
            string decimalSeparator = ci.NumberFormat.CurrencyDecimalSeparator;

            {

                line1 = line2 = "";

                if (theReader == null)
                    return;

                // reads line one.
                line1 = theReader.ReadLine();
                if (line1 != null)
                {
                    // trims line one so there is no white space 
                    line1 = line1.Trim();
                    
                    line1 = line1.Replace('.', decimalSeparator[0]);

                }
                // reads line two
                line2 = theReader.ReadLine();

                if (line2 != null)
                {
                    // trims line two so there is no white space
                    line2 = line2.Trim();
                    line2 = line2.Replace('.', decimalSeparator[0]);
                }
            }
        }


        //Interpretes line objects in the DXF file
        private void LineModule(StreamReader reader)		
        {
            // creates  astream
            Stream mystream;
            // initializes a savedialogbox
            SaveFileDialog savefileDialog1 = new SaveFileDialog();

            //  sets the save file type
            savefileDialog1.Filter = "txt files(*.txt)|*.txt";
            savefileDialog1.FilterIndex = 2;
            savefileDialog1.RestoreDirectory = true;

            // if OK button is pressed 
            if (savefileDialog1.ShowDialog() == DialogResult.OK)
            {
                if ((mystream = savefileDialog1.OpenFile()) != null)
                {
                    // creates a StreamWriter ready to write data to a file
                    StreamWriter writer = new StreamWriter(mystream);

                    string line1 = "0";
                    string line2 = "0";

                    double x1 = 0;
                    double y1 = 0;
                    double x2 = 0;
                    double y2 = 0;

                    do
                    {
                        // calls the getlinecouple function
                        GetLineCouple(reader, out line1, out line2);

                        switch (line1)
                        {
                                // chacks if line ne is 10
                            case "10":
                                // converts x1 to line2 as a double
                                x1 = Convert.ToDouble(line2);
                                // calls the function setmaxmin
                                SetMaxMin(x1, ref XMax, ref XMin);
                                break;
                            // chacks if line ne is 20
                            case "20":
                                // converts y1 to line2 as a double
                                y1 = Convert.ToDouble(line2);
                                SetMaxMin(y1, ref YMax, ref YMin);
                                PrintXYZ(x1, line2, writer);
                                break;
                            // chacks if line ne is 11
                            case "11":
                                // converts x2 to line2 as a double
                                x2 = Convert.ToDouble(line2);
                                SetMaxMin(x2, ref XMax, ref XMin);
                                break;
                            // chacks if line ne is 21
                            case "21":
                                // converts y2 to line2 as a double
                                y2 = Convert.ToDouble(line2);
                                SetMaxMin(y2, ref YMax, ref YMin);
                                PrintXYZ(x2, line2, writer);
                                break;
                            default:
                                break;
                        }
                    }
                    while (line2 != "EOF");


                    // if line 2 says EOF
                    if (line2 == "EOF")
                    {
                        // shows a message box with the file location of the saved fiel
                        MessageBox.Show("File converted\n File location is  " + savefileDialog1.FileName, "DXF converter",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                        // disposes the writer, stream and savedialogbox
                        writer.Dispose();
                        mystream.Dispose();
                        savefileDialog1.Dispose();
                        
                    }
                }
            }
        }

        // function for printing the x, y and z co-ordinates
        private static void PrintXYZ(double value, string lineValue, StreamWriter writer)
        {
            string xyz = (value + "," + lineValue + ",0.0");
            writer.WriteLine(xyz);
        }

        // function to set the max and min 
        private static void SetMaxMin(double value, ref double max, ref double min)
        {
            if (value > max)
            {
                max = value;
            }
            if (value < min)
            {
                min = value;
            }
        }


        #endregion
    }
}
