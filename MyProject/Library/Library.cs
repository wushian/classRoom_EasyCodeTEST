﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

public class Library
{
    public static List<SelectListItem> GetConditions(String select)
    {
        List<SelectListItem> list = new List<SelectListItem>();
        SelectListItem Item1 = new SelectListItem { Text = "Contains", Value = "Contains" };
        SelectListItem Item2 = new SelectListItem { Text = "Equals", Value = "Equals" };
        SelectListItem Item3 = new SelectListItem { Text = "Starts with...", Value = "Starts with..." };
        SelectListItem Item4 = new SelectListItem { Text = "More than...", Value = "More than..." };
        SelectListItem Item5 = new SelectListItem { Text = "Less than...", Value = "Less than..." };
        SelectListItem Item6 = new SelectListItem { Text = "Equal or more than...", Value = "Equal or more than..." };
        SelectListItem Item7 = new SelectListItem { Text = "Equal or less than...", Value = "Equal or less than..." };

        if (select == "Contains") { Item1.Selected = true; }
        else if (select == "Equals") { Item2.Selected = true; }
        else if (select == "Starts with...") { Item3.Selected = true; }
        else if (select == "More than...") { Item4.Selected = true; }
        else if (select == "Less than...") { Item5.Selected = true; }
        else if (select == "Equal or more than...") { Item6.Selected = true; }
        else if (select == "Equal or less than...") { Item7.Selected = true; }

        list.Add(Item1);
        list.Add(Item2);
        list.Add(Item3);
        list.Add(Item4);
        list.Add(Item5);
        list.Add(Item6);
        list.Add(Item7);

        return list.ToList();
    }

    public static List<SelectListItem> GetExports(String select)
    {
        List<SelectListItem> list = new List<SelectListItem>();
        SelectListItem Item1 = new SelectListItem { Text = "Pdf", Value = "Pdf" };
        SelectListItem Item2 = new SelectListItem { Text = "Excel", Value = "Excel" };
        SelectListItem Item3 = new SelectListItem { Text = "Word", Value = "Word" };

        if (select == "Pdf") { Item1.Selected = true; }
        else if (select == "Excel") { Item2.Selected = true; }
        else if (select == "Word") { Item3.Selected = true; }

        list.Add(Item1);
        list.Add(Item2);
        list.Add(Item3);

        return list.ToList();
    }

    public static List<SelectListItem> GetPageSizes()
    {
        var pagesizes = new[] 
                {
                     new SelectListItem { Text = "5", Value = "5" }
                    ,new SelectListItem { Text = "10", Value = "10" }
                    ,new SelectListItem { Text = "25", Value = "25" }
                    ,new SelectListItem { Text = "50", Value = "50" }
                    ,new SelectListItem { Text = "100", Value = "100" }
                    ,new SelectListItem { Text = "500", Value = "500" }
                };
        return pagesizes.ToList();
    }
    
    public static DataTable ToDataTable<T>(List<T> items)
    {
        DataTable dataTable = new DataTable(typeof(T).Name);

        //Get all the properties
        PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
        foreach (PropertyInfo prop in Props)
        {
            //Setting column names as Property names
            dataTable.Columns.Add(prop.Name);
        }
        foreach (T item in items)
        {
            var values = new object[Props.Length];
            for (int i = 0; i < Props.Length; i++)
            {
                //inserting property values to datatable rows
                values[i] = Props[i].GetValue(item, null);
            }
            dataTable.Rows.Add(values);
        }
        //put a breakpoint here and check datatable
        return dataTable;
    }

}