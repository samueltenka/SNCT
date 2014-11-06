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
        public ObservableCollection<Answer> Answers { get; private set; }
        
        public class Answer
        {
            public string Sentence { get; private set; }
            public double Score { get; private set; }
            public Answer(string sentence, double score) {
                Sentence = sentence;
                Score = score;
            }
            private Answer() {}
        }


        public MainPage()
        {
            this.InitializeComponent();
            // initialize this so we have an empty collection into which we can put results
            this.Results = new ObservableCollection<WebResult>();
            this.Answers = new ObservableCollection<Answer>();
        }

        async void searchBox_QuerySubmitted(SearchBox sender, SearchBoxQuerySubmittedEventArgs args)
        {
            string massive_text = String.Empty;

            // Get a JsonArray of paraphrased sentences
            var paraphrases = await SNCT.Rephrasing.GetParaphrases(sender.QueryText, 5);
            foreach (var query in paraphrases) {
                // get the string from the JsonArray
                string q = query.GetString();
                // Grab the first result (to be changed)
                this.Results.Add((await SNCT.BingProvider.GetResultsForQuery(q))[0]);
            }
            foreach (var result in this.Results) {
                // Add the extracted text to our giant blob
                massive_text += await SNCT.AlchemyProvider.URLGetText(result.Url);
            }
            // get an string answer from Sam's stuff :)
            var answer = Finder.answer(massive_text, sender.QueryText, 5);
            foreach (var a in answer)
            {
                this.Answers.Add(new Answer(a.Key, a.Value));
            }
            this.DataContext = this.Answers;
        }

        private void ListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var item = e.ClickedItem as WebResult;
            this.Frame.Navigate(typeof(SummarizedResultPage), item);
        }
    }
}
