using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SNCT {
    class Program {
        static void Main() {
            while(true) {
                String text = TextGetter.get_text();
                String query = TextGetter.get_query();
                foreach(KeyValuePair<String, double> result in Finder.answer(text, query, 5)) {
                    Console.WriteLine(result.Value);
                    Console.WriteLine(result.Key);
                    Console.WriteLine();
                    Console.WriteLine();
                }
            }
        }
    }
}

