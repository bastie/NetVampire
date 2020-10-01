/*
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */
using System;
using java = biz.ritter.javapi;
using org.apache.commons.compress.archivers;

namespace org.apache.commons.compress.archivers.zip {

    /**
     * Implements an input stream that can read Zip archives.
     * <p>
     * Note that {@link ZipArchiveEntry#getSize()} may return -1 if the DEFLATE algorithm is used, as the size information
     * is not available from the header.
     * <p>
     * The {@link ZipFile} class is preferred when reading from files.
     *  
     * @see ZipFile
     * @NotThreadSafe
     */
    public class ZipArchiveInputStream : ArchiveInputStream {

        private static readonly int SHORT = 2;
        private static readonly int WORD = 4;

        /**
         * The zip encoding to use for filenames and the file comment.
         */
        private readonly ZipEncoding zipEncoding;

        /**
         * Whether to look for and use Unicode extra fields.
         */
        private readonly bool useUnicodeExtraFields;

        private readonly java.io.InputStream inJ;

        private readonly java.util.zip.Inflater inf = new java.util.zip.Inflater(true);
        private readonly java.util.zip.CRC32 crc = new java.util.zip.CRC32();

        private readonly byte[] buf = new byte[ZipArchiveOutputStream.BUFFER_SIZE];

        private ZipArchiveEntry current = null;
        private bool closed = false;
        private bool hitCentralDirectory = false;
        private int readBytesOfEntry = 0, offsetInBuffer = 0;
        private int bytesReadFromStream = 0;
        private int lengthOfLastRead = 0;
        private bool hasDataDescriptor = false;
        private java.io.ByteArrayInputStream lastStoredEntry = null;

        private bool allowStoredEntriesWithDataDescriptor = false;

        private static readonly int LFH_LEN = 30;
        /*
          local file header signature     4 bytes  (0x04034b50)
          version needed to extract       2 bytes
          general purpose bit flag        2 bytes
          compression method              2 bytes
          last mod file time              2 bytes
          last mod file date              2 bytes
          crc-32                          4 bytes
          compressed size                 4 bytes
          uncompressed size               4 bytes
          file name length                2 bytes
          extra field length              2 bytes
        */

        public ZipArchiveInputStream(java.io.InputStream inputStream)  :this(inputStream, ZipEncodingHelper.UTF8, true) {}

        /**
         * @param encoding the encoding to use for file names, use null
         * for the platform's default encoding
         * @param useUnicodeExtraFields whether to use InfoZIP Unicode
         * Extra Fields (if present) to set the file names.
         */
        public ZipArchiveInputStream(java.io.InputStream inputStream,
                                     String encoding,
                                     bool useUnicodeExtraFields) : this(inputStream, encoding, useUnicodeExtraFields, false){}

        /**
         * @param encoding the encoding to use for file names, use null
         * for the platform's default encoding
         * @param useUnicodeExtraFields whether to use InfoZIP Unicode
         * Extra Fields (if present) to set the file names.
         * @param allowStoredEntriesWithDataDescriptor whether the stream
         * will try to read STORED entries that use a data descriptor
         * @since Apache Commons Compress 1.1
         */
        public ZipArchiveInputStream(java.io.InputStream inputStream,
                                     String encoding,
                                     bool useUnicodeExtraFields,
                                     bool allowStoredEntriesWithDataDescriptor) {
            zipEncoding = ZipEncodingHelper.getZipEncoding(encoding);
            this.useUnicodeExtraFields = useUnicodeExtraFields;
            inJ = new java.io.PushbackInputStream(inputStream, buf.Length);
            this.allowStoredEntriesWithDataDescriptor =
                allowStoredEntriesWithDataDescriptor;
        }

        public ZipArchiveEntry getNextZipEntry() //throws IOException 
        {
            if (closed || hitCentralDirectory) {
                return null;
            }
            if (current != null) {
                closeEntry();
            }
            byte[] lfh = new byte[LFH_LEN];
            try {
                readFully(lfh);
            } catch (java.io.EOFException ) {
                return null;
            }
            ZipLong sig = new ZipLong(lfh);
            if (sig.equals(ZipLong.CFH_SIG)) {
                hitCentralDirectory = true;
                return null;
            }
            if (!sig.equals(ZipLong.LFH_SIG)) {
                return null;
            }

            int off = WORD;
            current = new ZipArchiveEntry();

            int versionMadeBy = ZipShort.getValue(lfh, off);
            off += SHORT;
            current.setPlatform((versionMadeBy >> ZipFile.BYTE_SHIFT)
                                & ZipFile.NIBLET_MASK);

            GeneralPurposeBit gpFlag = GeneralPurposeBit.parse(lfh, off);
            bool hasUTF8Flag = gpFlag.usesUTF8ForNames();
            ZipEncoding entryEncoding =
                hasUTF8Flag ? ZipEncodingHelper.UTF8_ZIP_ENCODING : zipEncoding;
            hasDataDescriptor = gpFlag.usesDataDescriptor();
            current.setGeneralPurposeBit(gpFlag);

            off += SHORT;

            current.setMethod(ZipShort.getValue(lfh, off));
            off += SHORT;

            long time = ZipUtil.dosToJavaTime(ZipLong.getValue(lfh, off));
            current.setTime(time);
            off += WORD;

            if (!hasDataDescriptor) {
                current.setCrc(ZipLong.getValue(lfh, off));
                off += WORD;

                current.setCompressedSize(ZipLong.getValue(lfh, off));
                off += WORD;

                current.setSize(ZipLong.getValue(lfh, off));
                off += WORD;
            } else {
                off += 3 * WORD;
            }

            int fileNameLen = ZipShort.getValue(lfh, off);

            off += SHORT;

            int extraLen = ZipShort.getValue(lfh, off);
            off += SHORT;

            byte[] fileName = new byte[fileNameLen];
            readFully(fileName);
            current.setName(entryEncoding.decode(fileName));

            byte[] extraData = new byte[extraLen];
            readFully(extraData);
            current.setExtra(extraData);

            if (!hasUTF8Flag && useUnicodeExtraFields) {
                ZipUtil.setNameAndCommentFromExtraFields(current, fileName, null);
            }
            return current;
        }

        /** {@inheritDoc} */
        public override ArchiveEntry getNextEntry() //throws IOException 
        {
            return getNextZipEntry();
        }

        /**
         * Whether this class is able to read the given entry.
         *
         * <p>May return false if it is set up to use encryption or a
         * compression method that hasn't been implemented yet.</p>
         * @since Apache Commons Compress 1.1
         */
        public override bool canReadEntryData(ArchiveEntry ae) {
            if (ae is ZipArchiveEntry) {
                ZipArchiveEntry ze = (ZipArchiveEntry) ae;
                return ZipUtil.canHandleEntryData(ze)
                    && supportsDataDescriptorFor(ze);

            }
            return false;
        }

        public override int read(byte[] buffer, int start, int length) //throws IOException 
        {
            if (closed) {
                throw new java.io.IOException("The stream is closed");
            }
            if (inf.finished() || current == null) {
                return -1;
            }

            // avoid int overflow, check null buffer
            if (start <= buffer.Length && length >= 0 && start >= 0
                && buffer.Length - start >= length) {
                ZipUtil.checkRequestedFeatures(current);
                if (!supportsDataDescriptorFor(current)) {

                    throw new UnsupportedZipFeatureException(Feature.DATA_DESCRIPTOR,current);
                }

                if (current.getMethod() == ZipArchiveOutputStream.STORED) {
                    if (hasDataDescriptor) {
                        if (lastStoredEntry == null) {
                            readStoredEntry();
                        }
                        return lastStoredEntry.read(buffer, start, length);
                    }

                    int csize = (int) current.getSize();
                    if (readBytesOfEntry >= csize) {
                        return -1;
                    }
                    if (offsetInBuffer >= lengthOfLastRead) {
                        offsetInBuffer = 0;
                        if ((lengthOfLastRead = inJ.read(buf)) == -1) {
                            return -1;
                        }
                        count(lengthOfLastRead);
                        bytesReadFromStream += lengthOfLastRead;
                    }
                    int toRead = length > lengthOfLastRead
                        ? lengthOfLastRead - offsetInBuffer
                        : length;
                    if ((csize - readBytesOfEntry) < toRead) {
                        toRead = csize - readBytesOfEntry;
                    }
                    java.lang.SystemJ.arraycopy(buf, offsetInBuffer, buffer, start, toRead);
                    offsetInBuffer += toRead;
                    readBytesOfEntry += toRead;
                    crc.update(buffer, start, toRead);
                    return toRead;
                }

                if (inf.needsInput()) {
                    fill();
                    if (lengthOfLastRead > 0) {
                        bytesReadFromStream += lengthOfLastRead;
                    }
                }
                int read = 0;
                try {
                    read = inf.inflate(buffer, start, length);
                } catch (java.util.zip.DataFormatException e) {
                    throw new java.util.zip.ZipException(e.getMessage());
                }
                if (read == 0) {
                    if (inf.finished()) {
                        return -1;
                    } else if (lengthOfLastRead == -1) {
                        throw new java.io.IOException("Truncated ZIP file");
                    }
                }
                crc.update(buffer, start, read);
                return read;
            }
            throw new java.lang.ArrayIndexOutOfBoundsException();
        }

        public override void close() //throws IOException 
        {
            if (!closed) {
                closed = true;
                inJ.close();
            }
        }

        public override long skip(long value) //throws IOException 
        {
            if (value >= 0) {
                long skipped = 0;
                byte[] b = new byte[1024];
                while (skipped != value) {
                    long rem = value - skipped;
                    int x = read(b, 0, (int) (b.Length > rem ? rem : b.Length));
                    if (x == -1) {
                        return skipped;
                    }
                    skipped += x;
                }
                return skipped;
            }
            throw new java.lang.IllegalArgumentException();
        }

        /**
         * Checks if the signature matches what is expected for a zip file.
         * Does not currently handle self-extracting zips which may have arbitrary
         * leading content.
         * 
         * @param signature
         *            the bytes to check
         * @param length
         *            the number of bytes to check
         * @return true, if this stream is a zip archive stream, false otherwise
         */
        public static bool matches(byte[] signature, int length) {
            if (length < ZipArchiveOutputStream.LFH_SIG.Length) {
                return false;
            }

            return checksig(signature, ZipArchiveOutputStream.LFH_SIG) // normal file
                || checksig(signature, ZipArchiveOutputStream.EOCD_SIG); // empty zip
        }

        private static bool checksig(byte[] signature, byte[] expected){
            for (int i = 0; i < expected.Length; i++) {
                if (signature[i] != expected[i]) {
                    return false;
                }
            }
            return true;        
        }

        /**
         * Closes the current ZIP archive entry and positions the underlying
         * stream to the beginning of the next entry. All per-entry variables
         * and data structures are cleared.
         * <p>
         * If the compressed size of this entry is included in the entry header,
         * then any outstanding bytes are simply skipped from the underlying
         * stream without uncompressing them. This allows an entry to be safely
         * closed even if the compression method is unsupported.
         * <p>
         * In case we don't know the compressed size of this entry or have
         * already buffered too much data from the underlying stream to support
         * uncompression, then the uncompression process is completed and the
         * end position of the stream is adjusted based on the result of that
         * process.
         *
         * @throws IOException if an error occurs
         */
        private void closeEntry() //throws IOException 
        {
            if (closed) {
                throw new java.io.IOException("The stream is closed");
            }
            if (current == null) {
                return;
            }

            // Ensure all entry bytes are read
            if (bytesReadFromStream <= current.getCompressedSize()
                    && !hasDataDescriptor) {
                long remaining = current.getCompressedSize() - bytesReadFromStream;
                while (remaining > 0) {
                    long n = inJ.read(buf, 0, (int) java.lang.Math.min(buf.Length, remaining));
                    if (n < 0) {
                        throw new java.io.EOFException(
                                "Truncated ZIP entry: " + current.getName());
                    } else {
                        count(n);
                        remaining -= n;
                    }
                }
            } else {
                skip(java.lang.Long.MAX_VALUE);

                int inB = 0;
                if (current.getMethod() == ZipArchiveOutputStream.DEFLATED) {
                    inB = inf.getTotalIn();
                } else {
                    inB = readBytesOfEntry;
                }
                int diff = 0;

                // Pushback any required bytes
                if ((diff = bytesReadFromStream - inB) != 0) {
                    ((java.io.PushbackInputStream) inJ).unread(
                            buf,  lengthOfLastRead - diff, diff);
                    pushedBackBytes(diff);
                }
            }

            if (lastStoredEntry == null && hasDataDescriptor) {
                readDataDescriptor();
            }

            inf.reset();
            readBytesOfEntry = offsetInBuffer = bytesReadFromStream =
                lengthOfLastRead = 0;
            crc.reset();
            current = null;
            lastStoredEntry = null;
        }

        private void fill() //throws IOException
        {
            if (closed) {
                throw new java.io.IOException("The stream is closed");
            }
            if ((lengthOfLastRead = inJ.read(buf)) > 0) {
                count(lengthOfLastRead);
                inf.setInput(buf, 0, lengthOfLastRead);
            }
        }

        private void readFully(byte[] b) //throws IOException 
        {
            int countJ = 0, x = 0;
            while (countJ != b.Length) {
                countJ += x = inJ.read(b, countJ, b.Length - countJ);
                if (x == -1) {
                    throw new java.io.EOFException();
                }
                count(x);
            }
        }

        private void readDataDescriptor() //throws IOException 
        {
            byte[] b = new byte[WORD];
            readFully(b);
            ZipLong val = new ZipLong(b);
            if (ZipLong.DD_SIG.equals(val)) {
                // data descriptor with signature, skip sig
                readFully(b);
                val = new ZipLong(b);
            }
            current.setCrc(val.getValue());
            readFully(b);
            current.setCompressedSize(new ZipLong(b).getValue());
            readFully(b);
            current.setSize(new ZipLong(b).getValue());
        }

        /**
         * Whether this entry requires a data descriptor this library can work with.
         *
         * @return true if allowStoredEntriesWithDataDescriptor is true,
         * the entry doesn't require any data descriptor or the method is
         * DEFLATED.
         */
        private bool supportsDataDescriptorFor(ZipArchiveEntry entry) {
            return allowStoredEntriesWithDataDescriptor ||
                !entry.getGeneralPurposeBit().usesDataDescriptor()
                || entry.getMethod() == ZipArchiveEntry.DEFLATED;
        }

        /**
         * Caches a stored entry that uses the data descriptor.
         *
         * <ul>
         *   <li>Reads a stored entry until the signature of a local file
         *     header, central directory header or data descriptor has been
         *     found.</li>
         *   <li>Stores all entry data in lastStoredEntry.</p>
         *   <li>Rewinds the stream to position at the data
         *     descriptor.</li>
         *   <li>reads the data descriptor</li>
         * </ul>
         *
         * <p>After calling this method the entry should know its size,
         * the entry's data is cached and the stream is positioned at the
         * next local file or central directory header.</p>
         */
        private void readStoredEntry() //throws IOException 
        {
            java.io.ByteArrayOutputStream bos = new java.io.ByteArrayOutputStream();
            byte[] LFH = ZipLong.LFH_SIG.getBytes();
            byte[] CFH = ZipLong.CFH_SIG.getBytes();
            byte[] DD = ZipLong.DD_SIG.getBytes();
            int off = 0;
            bool done = false;

            while (!done) {
                int r = inJ.read(buf, off, ZipArchiveOutputStream.BUFFER_SIZE - off);
                if (r <= 0) {
                    // read the whole archive without ever finding a
                    // central directory
                    throw new java.io.IOException("Truncated ZIP file");
                }
                if (r + off < 4) {
                    // buf is too small to check for a signature, loop
                    off += r;
                    continue;
                }

                int readTooMuch = 0;
                for (int i = 0; !done && i < r - 4; i++) {
                    if (buf[i] == LFH[0] && buf[i + 1] == LFH[1]) {
                        if ((buf[i + 2] == LFH[2] && buf[i + 3] == LFH[3])
                            || (buf[i] == CFH[2] && buf[i + 3] == CFH[3])) {
                            // found a LFH or CFH:
                            readTooMuch = off + r - i - 12 /* dd without signature */;
                            done = true;
                        }
                        else if (buf[i + 2] == DD[2] && buf[i + 3] == DD[3]) {
                            // found DD:
                            readTooMuch = off + r - i;
                            done = true;
                        }
                        if (done) {
                            // * push back bytes read in excess as well as the data
                            //   descriptor
                            // * copy the remaining bytes to cache
                            // * read data descriptor
                            ((java.io.PushbackInputStream) inJ).unread(buf, off + r - readTooMuch, readTooMuch);
                            bos.write(buf, 0, i);
                            readDataDescriptor();
                        }
                    }
                }
                if (!done) {
                    // worst case we've read a data descriptor without a
                    // signature (12 bytes) plus the first three bytes of
                    // a LFH or CFH signature
                    // save the last 15 bytes in the buffer, cache
                    // anything in front of that, read on
                    if (off + r > 15) {
                        bos.write(buf, 0, off + r - 15);
                        java.lang.SystemJ.arraycopy(buf, off + r - 15, buf, 0, 15);
                        off = 15;
                    } else {
                        off += r;
                    }
                }
            }

            byte[] b = bos.toByteArray();
            lastStoredEntry = new java.io.ByteArrayInputStream(b);
        }
    }
}