using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

using SNCT;

namespace TestApp
{
    public sealed partial class MainPage : Page
    {
        public ObservableCollection<WebResult> Results { get; private set; }
        //public ObservableCollection<Answer> Answers { get; private set; }
        public string Answer { get; private set; }
        
        //public class Answer
        //{
        //    public string Sentence { get; private set; }
        //    public double Score { get; private set; }
        //    public Answer(string sentence, double score) {
        //        Sentence = sentence;
        //        Score = score;
        //    }
        //    private Answer() {}
        //}


        public MainPage()
        {
            this.InitializeComponent();
            // initialize this so we have an empty collection into which we can put results
            this.Results = new ObservableCollection<WebResult>();
            //this.Answers = new ObservableCollection<Answer>();
        }

        async void searchBox_QuerySubmitted(SearchBox sender, SearchBoxQuerySubmittedEventArgs args)
        {
            string massive_text = String.Empty;
            SBSearchStep1.Begin();
            SBSearchStep1.Completed += (_, __) =>
            {
                this.Results.Clear();
                this.Answer = String.Empty;
                //this.Answers.Clear();
            };

            // Get a JsonArray of paraphrased sentences
            var paraphrases = await SNCT.Rephrasing.GetParaphrases(sender.QueryText, 5);
            await System.Threading.Tasks.Task.Delay(1000);
            SBSearchStep2.Begin();
            var paraphrases_list = new List<string>();
            foreach (var query in paraphrases) {
                // get the string from the JsonArray
                string q = query.GetString();
                paraphrases_list.Add(q);
                // Grab the first result (to be changed)
                this.Results.Add((await SNCT.BingProvider.GetResultsForQuery(q))[0]);
            }
            SBSearchStep3.Begin();
            for (var i = 0; i < this.Results.Count; i++)
            {
                // Add the extracted text to our giant blob
                massive_text += await SNCT.AlchemyProvider.URLGetText(this.Results[i].Url);
            }
            SBSearchStep4.Begin();
            // get an string answer from Sam's stuff :)
            var answer = await Finder.answer(massive_text, paraphrases_list.ToArray(), 5);
            // get the ordereddict to sort these for us--it sorts by increasing Key (aka the sentence score [double])
            var ordered_answers = new SortedDictionary<double, string>();
            foreach (var a in answer)
            {
                // we actually want the higher scores to be first, so lets fudge it instead of writing our own comparator
                ordered_answers.Add(100.0 - a.Value,  a.Key);
            }

            this.Answer = ordered_answers.ElementAt(0).Value + "\n\n" + ordered_answers.ElementAt(1).Value;
            
            this.DataContext = this.Answer;
            ShowResultsStoryboard.Begin();
        }
    }
}
