using PlanePlanning.Extensions;
using System.Text;

namespace PlanePlanning.Models
{
    public class Airplane
    {
        public Row[] rows { get; set; } = new Row[34];

        public Airplane()
        {
            initialize();
        }

        private void initialize()
        {
            rows[0] = new Row(1, 1);

            Enumerable
                .Range(1, 33)
                .ForEach(i => rows[i] = new Row(3, 3));
        }

        public string getLayout()
        {
            StringBuilder sb = new StringBuilder();

            this.rows.ForEach(r => sb.AppendLine(getRowAsString(r)));
            Console.WriteLine(sb.ToString());
            return sb.ToString();
        }

        private string getRowAsString(Row r)
        {
            string rowAsString = string.Join("|", addPadding(r.seats[0])).PadLeft(29, ' ') + "   ||||||   " + string.Join("|", addPadding(r.seats[1])).PadRight(29, ' ');
            return rowAsString;
        }

        private List<string> addPadding(string[] values)
        {
            return values.ToList().ConvertAll(v => v == null ? "         " : v.PadRight(9, ' '));
        }
    }

}
