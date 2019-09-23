using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Java.Lang;

namespace AplicacionMovil.Adapters
{
    public class ListViewGroupedAdapter : BaseAdapter, ISectionIndexer
    {
        private List<Java.Lang.Object> m_HeaderTitles;
        private int m_ItemCount = 0;
        private Dictionary<string, int> m_IndexDictionary = new Dictionary<string, int>();

        Dictionary<string, IAdapter> sections = new Dictionary<string, IAdapter>();
        ArrayAdapter<string> headers;
        const int TypeSectionHeader = 0;
        public int cantidadItems { get; set; }

        public void AddSection(string section, IAdapter adapter)
        {
            m_HeaderTitles.Add(section);
            headers.Add(section);
            sections.Add(section, adapter);
            m_IndexDictionary.Add(section, m_ItemCount);
            m_ItemCount += (adapter.Count + 1);
        }

        public ListViewGroupedAdapter(Context context)
        {
            headers = new ArrayAdapter<string>(context, Resource.Layout.ListHeader);
            m_HeaderTitles = new List<Java.Lang.Object>();
        }

        public override int Count
        {
            get { return sections.Values.Sum(x => x.Count + 1); }
            //get { return m_ItemCount; }
        }

        public override int ViewTypeCount
        {
            get { return 1 + sections.Values.Sum(x => x.ViewTypeCount); }
            //get { return cantidadItems; }
            //get { return 1; }
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            int sectionnum = 0;
            foreach (var section in sections.Keys)
            {
                var adapter = sections[section];
                int size = adapter.Count + 1;

                if (position == 0)
                    return headers.GetView(sectionnum, convertView, parent);
                if (position < size)
                    return adapter.GetView(position - 1, convertView, parent);

                position -= size;
                sectionnum++;
            }
            return null;
            /*var view = convertView ?? activity.LayoutInflater.Inflate(Resource.Layout.listviewCliente, parent, false);
            var textNombre = view.FindViewById<TextView>(Resource.Id.textListViewNombreCliente);
            var textRut = view.FindViewById<TextView>(Resource.Id.textListViewRUTCliente);
            textNombre.Text = clientes[position].NombreCompleto;
            textRut.Text = "RUT: " + clientes[position].Rut;
            return view;*/
        }

        public override int GetItemViewType(int position)
        {
            int type = 1;
            foreach (var section in sections.Keys)
            {
                var adapter = sections[section];
                int size = adapter.Count + 1;

                if (position == 0)
                    return TypeSectionHeader;
                if (position < size)
                    return type + adapter.GetItemViewType(position - 1);

                position -= size;
                type += adapter.ViewTypeCount;
            }
            return -1;
        }

        public override bool AreAllItemsEnabled()
        {
            return false;
        }

        public override bool IsEnabled(int position)
        {
            return (GetItemViewType(position) != TypeSectionHeader);
        }

        public override Java.Lang.Object GetItem(int position)
        {
            int op = position;
            foreach (var section in sections.Keys)
            {
                var adapter = sections[section];
                int size = adapter.Count + 1;
                if (position == 0)
                    return section;
                if (position < size)
                    return adapter.GetItem(position - 1);
                position -= size;
            }
            return null;
        }

        public int GetPositionForSection(int sectionIndex)
        {
            var letter = headers.GetItem(sectionIndex);
            return m_IndexDictionary[letter];
        }

        public int GetSectionForPosition(int position)
        {
            return 1;
        }

        public Java.Lang.Object[] GetSections()
        {
            return m_HeaderTitles.ToArray();
        }
    }
}