using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace CSVParser
{
    internal class StreamEnumerator : IEnumerable<string>, IDisposable
    {
        StreamReader sr;
        internal StreamEnumerator(StreamReader sr) { this.sr = sr; }

        IEnumerator<string> IEnumerable<string>.GetEnumerator()
        {
            while (sr.Peek() != -1)
            {
                yield return sr.ReadLine();
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        ~StreamEnumerator()
        {
            this.Dispose(false);
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected void Dispose(bool disposing)
        {
            sr.Close();
            sr = null;
        }
    }
}
