using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ItzWarty.Database
{
    public enum ComparisonOperator
    {
        Is,
        IsNot,
        LessThan,
        LessThanEqualTo,
        GreaterThan,
        GreaterThanEqualTo,
        StartsWith
    }
    public class wDBRowCollection
    {
        public List<wDBTableRow> rows = new List<wDBTableRow>();
        public wDBRowCollection(){}
        public wDBRowCollection(List<wDBTableRow> rows)
        {
            this.rows = rows;
        }
        public wDBRowCollection Where(string what, ComparisonOperator op, object operand)
        {
            string operandTwo = "";
            if (operand is int)
                operandTwo = ((int)operand).ToString();
            else if (operand is string)
                operandTwo = (string)operand;
            else if (operand is bool)
                operandTwo = ((bool)operand) ? "1" : "0";

            wDBRowCollection newCollection = new wDBRowCollection();
            for (int i = 0; i < this.rows.Count; i++)
            {
                string operandOne = this.rows[i][what];
                double opOne = 0, opTwo = 0;
                double.TryParse(operandOne, out opOne);
                double.TryParse(operandTwo, out opTwo);
                switch(op)
                {
                    case ComparisonOperator.Is:
                        if(operandOne == operandTwo)
                            newCollection.rows.Add(this.rows[i]);
                        break;
                    case ComparisonOperator.IsNot:
                        if(operandOne != operandTwo)
                            newCollection.rows.Add(this.rows[i]);
                        break;
                    case ComparisonOperator.LessThan:
                        if(opOne < opTwo)
                            newCollection.rows.Add(this.rows[i]);
                        break;
                    case ComparisonOperator.LessThanEqualTo:
                        if(opOne <= opTwo)
                            newCollection.rows.Add(this.rows[i]);
                        break;
                    case ComparisonOperator.GreaterThan:
                        if(opOne > opTwo)
                            newCollection.rows.Add(this.rows[i]);
                        break;
                    case ComparisonOperator.GreaterThanEqualTo:
                        if(opOne >= opTwo)
                            newCollection.rows.Add(this.rows[i]);
                        break;
                    case ComparisonOperator.StartsWith:
                    {
                        if (operandOne.StartsWith(operandTwo))
                        {
                            newCollection.rows.Add(this.rows[i]);
                        }
                        break;
                    }
                }                    
            }
            return newCollection;
        }
        public int Count
        {
            get
            {
                return this.rows.Count;
            }
        }
        public wDBTableRow this[int i]
        {
            get
            {
                if (this.rows.Count >= i) return null;
                return this.rows[i];
            }
            set
            {
                this.rows[i] = value;
            }
        }
    }
}
