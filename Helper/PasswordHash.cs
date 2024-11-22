using System;
using System.Security.Cryptography;

namespace WebShop.Helper
{
    /// <summary>
    /// Die PasswordHash-Klasse bietet Funktionen zum Erstellen und Verifizieren von Passwort-Hashes.
    /// Sie verwendet dabei Salt und Hashing-Algorithmen, um die Sicherheit der Passwörter zu gewährleisten.
    /// </summary>
    public sealed class PasswordHash
    {
        // Konstanten für die Größe des Salts, die Größe des Hashes und die Anzahl der Iterationen
        const int SaltSize = 16, HashSize = 20, HashIter = 10000;

        // Private Felder für Salt und Hash
        readonly byte[] _salt, _hash;

        /// <summary>
        /// Erstellt eine neue Instanz der PasswordHash-Klasse und generiert einen Salt und Hash für das angegebene Passwort.
        /// </summary>
        /// <param name="password">Das Passwort, das gehasht werden soll.</param>
        public PasswordHash(string password)
        {
            // Generiert einen neuen Salt
            new RNGCryptoServiceProvider().GetBytes(_salt = new byte[SaltSize]);
            // Generiert den Hash basierend auf dem Passwort und dem Salt
            _hash = new Rfc2898DeriveBytes(password, _salt, HashIter).GetBytes(HashSize);
        }

        /// <summary>
        /// Erstellt eine neue Instanz der PasswordHash-Klasse basierend auf einem vorhandenen Hash-Byte-Array.
        /// </summary>
        /// <param name="hashBytes">Das Byte-Array, das den Salt und den Hash enthält.</param>
        public PasswordHash(byte[] hashBytes)
        {
            // Extrahiert den Salt aus dem Byte-Array
            Array.Copy(hashBytes, 0, _salt = new byte[SaltSize], 0, SaltSize);
            // Extrahiert den Hash aus dem Byte-Array
            Array.Copy(hashBytes, SaltSize, _hash = new byte[HashSize], 0, HashSize);
        }

        /// <summary>
        /// Erstellt eine neue Instanz der PasswordHash-Klasse basierend auf einem vorhandenen Salt und Hash.
        /// </summary>
        /// <param name="salt">Das Byte-Array, das den Salt enthält.</param>
        /// <param name="hash">Das Byte-Array, das den Hash enthält.</param>
        public PasswordHash(byte[] salt, byte[] hash)
        {
            // Kopiert den Salt in das private Feld
            Array.Copy(salt, 0, _salt = new byte[SaltSize], 0, SaltSize);
            // Kopiert den Hash in das private Feld
            Array.Copy(hash, 0, _hash = new byte[HashSize], 0, HashSize);
        }

        /// <summary>
        /// Gibt den Salt und den Hash als ein zusammengefügtes Byte-Array zurück.
        /// </summary>
        /// <returns>Ein Byte-Array, das den Salt und den Hash enthält.</returns>
        public byte[] ToArray()
        {
            // Erstellt ein neues Byte-Array, das den Salt und den Hash enthält
            byte[] hashBytes = new byte[SaltSize + HashSize];
            // Kopiert den Salt in das Byte-Array
            Array.Copy(_salt, 0, hashBytes, 0, SaltSize);
            // Kopiert den Hash in das Byte-Array
            Array.Copy(_hash, 0, hashBytes, SaltSize, HashSize);
            return hashBytes;
        }

        /// <summary>
        /// Gibt eine Kopie des Salts zurück.
        /// </summary>
        public byte[] Salt { get { return (byte[])_salt.Clone(); } }

        /// <summary>
        /// Gibt eine Kopie des Hashes zurück.
        /// </summary>
        public byte[] Hash { get { return (byte[])_hash.Clone(); } }

        /// <summary>
        /// Verifiziert, ob das angegebene Passwort mit dem gespeicherten Hash übereinstimmt.
        /// </summary>
        /// <param name="password">Das zu überprüfende Passwort.</param>
        /// <returns>True, wenn das Passwort übereinstimmt, andernfalls False.</returns>
        public bool Verify(string password)
        {
            // Generiert einen Test-Hash basierend auf dem angegebenen Passwort und dem gespeicherten Salt
            byte[] test = new Rfc2898DeriveBytes(password, _salt, HashIter).GetBytes(HashSize);
            // Vergleicht den Test-Hash mit dem gespeicherten Hash
            for (int i = 0; i < HashSize; i++)
                if (test[i] != _hash[i])
                    return false;
            return true;
        }
    }
}