using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;

namespace Lab1
{
    class MainWindowViewModel : INotifyPropertyChanged
    {
        static public List<string> Methods
        {
            get
            {
                return new List<string>() { "Caesars cipher", "Polybius square cipher", "Tritemius cipher", "Vigenere cipher", "Rearrangement cipher", "Simple frequency analysis" };
            }
            set
            {
                ;
            }
        }
        private string selectedMethod = Methods[0];
        public string SelectedMethod
        {
            get { return selectedMethod; }
            set
            {
                selectedMethod = value;
                OnPropertyChanged("SelectedMethod");
            }
        }
        private string key = "";
        public string Key
        {
            get { return key; }
            set
            {
                key = value;
                OnPropertyChanged("Key");
            }
        }
        private string sourceText = "";
        public string SourceText
        {
            get { return sourceText; }
            set
            {
                sourceText = value;
                OnPropertyChanged("SourceText");
            }
        }
        private string modifiedText = "";
        public string ModifiedText
        {
            get { return modifiedText; }
            set
            {
                modifiedText = value;
                OnPropertyChanged("ModifiedText");
            }
        }

        private RelayCommand encodeCommand;
        public RelayCommand EncodeCommand
        {
            get { return encodeCommand; }
            set
            {
                ;
            }
        }
        private RelayCommand decodeCommand;
        public RelayCommand DecodeCommand
        {
            get { return decodeCommand; }
            set
            {
                ;
            }
        }
        private RelayCommand loadFromFileCommand;
        public RelayCommand LoadFromFileCommand
        {
            get { return loadFromFileCommand; }
            set
            {
                ;
            }
        }
        private RelayCommand saveToFileCommand;
        public RelayCommand SaveToFileCommand
        {
            get { return saveToFileCommand; }
            set
            {
                ;
            }
        }

        public MainWindowViewModel()
        {
            encodeCommand = new RelayCommand(Encode);
            decodeCommand = new RelayCommand(Decode);
            loadFromFileCommand = new RelayCommand(LoadFromFile);
            saveToFileCommand = new RelayCommand(SaveToFile);
        }

        void Encode(object obj)
        {
            EncodeOrDecode(true);
        }

        void Decode(object obj)
        {
            EncodeOrDecode(false);
        }

        void EncodeOrDecode(bool encode = true)
        {
            switch(SelectedMethod)
            {
                case "Caesars cipher":
                    {
                        int shift;
                        if (Int32.TryParse(Key, out shift))
                            ModifiedText = BaseCiphers.CaesarsCipherUniversal(SourceText, shift, encode);
                        else
                        {
                            MessageBox.Show("The key must be an integer value!");
                            ModifiedText = "";
                        }
                        break;
                    }
                case "Polybius square cipher":
                    {
                        ModifiedText = BaseCiphers.PolybiusSquareCipher(SourceText, Key, encode);
                        break;
                    }
                case "Tritemius cipher":
                    {
                        int shift;
                        if (Int32.TryParse(Key, out shift))
                            ModifiedText = BaseCiphers.TritemiusCipher(SourceText, shift, encode);
                        else
                        {
                            MessageBox.Show("The key must be an integer value!");
                            ModifiedText = "";
                        }

                        break;
                    }
                case "Vigenere cipher":
                    {
                        ModifiedText = BaseCiphers.VigenereCipher(SourceText, Key, encode);
                        break;
                    }
                case "Rearrangement cipher":
                    {
                        ModifiedText = BaseCiphers.RearrangementCipherByKey(SourceText, Key, encode);
                        break;
                    }
                case "Simple frequency analysis":
                    {
                        if (encode) ModifiedText = "";
                        else ModifiedText = BaseCiphers.SimpleFrequencyAnalysis(SourceText);
                        break;
                    }
            }
        }

        void LoadFromFile(object obj)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.FileName = "Input";
            dialog.DefaultExt = ".txt";

            if (dialog.ShowDialog() == true)
            {
                try
                {
                    using (StreamReader FileRead = new StreamReader(dialog.FileName))
                        SourceText = FileRead.ReadToEnd();
                }
                catch (Exception)
                {
                    MessageBox.Show("Something went wrong while reading a file...");
                }
            }
        }

        void SaveToFile(object obj)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.FileName = "Output";
            dialog.DefaultExt = ".txt";

            if (dialog.ShowDialog() == true)
            {
                try
                {
                    using (StreamWriter FileWriter = new StreamWriter(dialog.FileName))
                        FileWriter.Write(ModifiedText);
                }
                catch (Exception)
                {
                    MessageBox.Show("Something went wrong while writing to file...");
                }
            }
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

        #endregion
    }
}
