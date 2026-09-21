using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RPA.ClaimStatements.Web.Models
{
    public static class SelectionValues
    {
        public static List<SelectListItem> StatementTypes()
        {
            List<SelectListItem> list = new List<SelectListItem>();

            list.Add(new SelectListItem() { Text = "All", Value = "ALL" });
            list.Add(new SelectListItem() { Text = "BPS Core", Value = "BPS" });
            list.Add(new SelectListItem() { Text = "BPS Cross Border", Value = "XB" });

            return list;
        }

        public static List<SelectListItem> ListValues()
        {
            List<SelectListItem> list = new List<SelectListItem>();

            list.Add(new SelectListItem() { Text = "Active", Value = "Active" });
            list.Add(new SelectListItem() { Text = "Inactive", Value = "Inactive" });

            return list;
        }

        public static List<SelectListItem> CreateOptions()
        {
            List<SelectListItem> list = new List<SelectListItem>();

            list.Add(new SelectListItem { Text = "Record", Value = "Record" });
            list.Add(new SelectListItem { Text = "Bulk", Value = "Bulk" });
            
            return list;
        }

        public static List<SelectListItem> BulkDirections()
        {
            List<SelectListItem> list = new List<SelectListItem>();

            list.Add(new SelectListItem { Text = "Add", Value = "Add" });
            list.Add(new SelectListItem { Text = "Remove", Value = "Remove" });            

            return list;
        }

        internal static List<SelectListItem> GetSchemeYears()
        {
            List<SelectListItem> list = new List<SelectListItem>();
            
            for (int i = 2015; i < (DateTime.Now.Year + 2); i++)
            {
                list.Add(new SelectListItem() { Text = i.ToString(), Value = i.ToString() });
            }

            return list;
        }
    }
}