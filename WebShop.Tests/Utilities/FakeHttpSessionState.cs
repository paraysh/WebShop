//Bearbeiter: Yusuf Can Sönmez
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.SessionState;

namespace MvcFakes
{
    // Diese Klasse stellt einen gefälschten HttpSessionState für Unit-Tests bereit.
    public class FakeHttpSessionState : HttpSessionStateBase
    {
        // Privates Feld zur Speicherung der Session-Items.
        private readonly SessionStateItemCollection _sessionItems;

        // Konstruktor, der die Session-Items initialisiert.
        public FakeHttpSessionState(SessionStateItemCollection sessionItems)
        {
            _sessionItems = sessionItems;
        }

        // Methode zum Hinzufügen eines Elements zur Session.
        public override void Add(string name, object value)
        {
            _sessionItems[name] = value;
        }

        // Eigenschaft, die die Anzahl der Elemente in der Session zurückgibt.
        public override int Count
        {
            get
            {
                return _sessionItems.Count;
            }
        }

        // Methode zum Abrufen eines Enumerators für die Session-Items.
        public override IEnumerator GetEnumerator()
        {
            return _sessionItems.GetEnumerator();
        }

        // Eigenschaft, die die Schlüssel der Session-Items zurückgibt.
        public override NameObjectCollectionBase.KeysCollection Keys
        {
            get
            {
                return _sessionItems.Keys;
            }
        }

        // Indexer zum Abrufen oder Festlegen eines Session-Items anhand des Namens.
        public override object this[string name]
        {
            get
            {
                return _sessionItems[name];
            }
            set
            {
                _sessionItems[name] = value;
            }
        }

        // Indexer zum Abrufen oder Festlegen eines Session-Items anhand des Indexes.
        public override object this[int index]
        {
            get
            {
                return _sessionItems[index];
            }
            set
            {
                _sessionItems[index] = value;
            }
        }

        // Methode zum Entfernen eines Elements aus der Session.
        public override void Remove(string name)
        {
            _sessionItems.Remove(name);
        }
    }
}