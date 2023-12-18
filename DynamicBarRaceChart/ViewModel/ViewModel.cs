using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DynamicBarRaceChart
{
    public class ViewModel : INotifyPropertyChanged
    {
        int count = 1;

        private List<List<Model>> data1;

        private bool canStopTimer;

        private string progressString = "Start";
        public string ProgressString
        {
            get
            {
                return progressString;
            }
            set
            {
                progressString = value;
                OnPropertyChanged(nameof(ProgressString));
            }
        }

        private double year;
        public double Year
        {
            get
            {
                return year;
            }
            set
            {
                year = value;
                OnPropertyChanged(nameof(Year));
            }
        }

        private List<Model> data;
        public List<Model> Data
        {
            get
            {
                return data;
            }
            set
            {
                data = value;
                OnPropertyChanged(nameof(Data));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public List<Brush> DefaultBrushes
        {
            get; set;
        }

        public double StartYear { get; set; }

        public double EndYear { get; set; }

        public ViewModel()
        {

            var models = new List<Model>(ReadCSV());
            data1 = new List<List<Model>>();
            StartYear = models.First<Model>().XValue;
            EndYear = models.Last<Model>().XValue;
            int count = 0;
            var previousData = new List<Model>();
            for (double i = StartYear; i <= EndYear; i++)
            {
                IEnumerable<Model> data = models.Where(x => x.XValue == i).OrderByDescending(x => x.YValue).Take(11);
                data1.Insert(count, UpdateDataIndex(i, previousData, data));
                count++;
                previousData = data.ToList();
            }

            Data = data1[0];
            Year = data1[0].First().XValue;

        }

        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }



        public IEnumerable<Model> ReadCSV()
        {
            Assembly executingAssembly = typeof(App).GetTypeInfo().Assembly;
            Stream inputStream = executingAssembly.GetManifestResourceStream("DynamicBarRaceChart.Resources.Raw.populationdata.csv");

            if (inputStream == null)
            {
                return new List<Model>();
            }

            string? line;
            List<string> lines = new List<string>();

            using StreamReader reader = new StreamReader(inputStream);
            while ((line = reader.ReadLine()) != null)
            {
                lines.Add(line);
            }
            int index = -1;
            double previousYear = 0;
            string previousString = string.Empty;

            return lines.Select(line =>
            {
                string[] data = line.Split(',');
                string currentString = data[0];
                double currentYear = Convert.ToDouble(data[1]);
                if (currentString != previousString)
                {
                    if (currentYear == previousYear)
                    {
                        index = -1;
                    }
                    index = index + 1;
                    previousString = currentString;
                }

                return new Model(index, data[0], currentYear, Convert.ToDouble(data[2]));
            });
        }


        private List<Model> UpdateDataIndex(double currentYear, List<Model> previousData, IEnumerable<Model> currentIndex)
        {
            var updatedData = new List<Model>(previousData);
            if (previousData != null && previousData.Count > 0)
            {
                for (int i = 0; i < currentIndex.Count(); i++)
                {
                    var item = currentIndex.ElementAt(i);
                    var previousItem = previousData[i];
                    item.Ranking = i;
                    if (item.XString == previousItem.XString)
                    {
                        updatedData[i] = (item);
                    }
                    else
                    {
                        for (int j = i; j < updatedData.Count; j++)
                        {
                            var previousItem1 = previousData[j];
                            if (item.XString == previousItem1.XString)
                            {
                                var previousvalue = previousData[i].XString;
                                updatedData[j] = item;
                                var currentitem = currentIndex.Where(x => x.XString == previousvalue);
                                if (currentitem.Count() > 0)
                                    updatedData[i] = currentitem.First();

                                break;
                            }
                        }
                    }
                }

            }
            else
            {
                for (int i = 0; i < currentIndex.Count(); i++)
                {
                    var item = currentIndex.ElementAt(i);
                    item.Ranking = i;
                    updatedData.Add(item);
                }
            }
            return updatedData;
        }

        private bool UpdateData()
        {
            if (data1.Count < count + 1)
            {
                ProgressString = "Start";
                count = 0;
                return false;
            }

            if (canStopTimer) return false;

            MainThread.InvokeOnMainThreadAsync(() =>
            {
                Data = data1[count];
                Year = data1[count].First().XValue;
                count++;
            });

            return true;
        }

        public void StopTimer()
        {
            canStopTimer = true;
            ProgressString = "Start";
        }

        public async void StartTimer()
        {
            ProgressString = "Pause";
            await Task.Delay(500);

            if (Application.Current != null)
                Application.Current.Dispatcher.StartTimer(new TimeSpan(0, 0, 0, 1, 500), UpdateData);

            canStopTimer = false;
        }
    }
}
