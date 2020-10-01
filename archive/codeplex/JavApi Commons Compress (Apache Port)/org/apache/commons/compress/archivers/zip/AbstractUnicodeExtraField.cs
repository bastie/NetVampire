/*
 *  Licensed to the Apache Software Foundation (ASF) under one or more
 *  contributor license agreements.  See the NOTICE file distributed with
 *  this work for additional information regarding copyright ownership.
 *  The ASF licenses this file to You under the Apache License, Version 2.0
 *  (the "License"); you may not use this file except in compliance with
 *  the License.  You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 *  Unless required by applicable law or agreed to in writing, software
 *  distributed under the License is distributed on an "AS IS" BASIS,
 *  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *  See the License for the specific language governing permissions and
 *  limitations under the License.
 *
 */
using System;
using java = biz.ritter.javapi;

namespace org.apache.commons.compress.archivers.zip {

    /**
     * A common base class for Unicode extra information extra fields.
     * @NotThreadSafe
     */
    public abstract class AbstractUnicodeExtraField : ZipExtraField {
        private long nameCRC32;
        private byte[] unicodeName;
        private byte[] data;

        protected AbstractUnicodeExtraField() {
        }

        public abstract ZipShort getHeaderId();
        /**
         * Assemble as unicode extension from the name/comment and
         * encoding of the orginal zip entry.
         * 
         * @param text The file name or comment.
         * @param bytes The encoded of the filename or comment in the zip
         * file.
         * @param off The offset of the encoded filename or comment in
         * <code>bytes</code>.
         * @param len The length of the encoded filename or commentin
         * <code>bytes</code>.
         */
        protected AbstractUnicodeExtraField(String text, byte[] bytes, int off, int len) {
            init(text, bytes, off, len);
        }

        protected void init(String text, byte[] bytes, int off, int len)
        {
            java.util.zip.CRC32 crc32 = new java.util.zip.CRC32();
            crc32.update(bytes, off, len);
            nameCRC32 = crc32.getValue();

            try
            {
                unicodeName = text.getBytes("UTF-8");
            }
            catch (java.io.UnsupportedEncodingException e)
            {
                throw new java.lang.RuntimeException("FATAL: UTF-8 encoding not supported.", e);
            }

        }

        /**
         * Assemble as unicode extension from the name/comment and
         * encoding of the orginal zip entry.
         * 
         * @param text The file name or comment.
         * @param bytes The encoded of the filename or comment in the zip
         * file.
         */
        protected AbstractUnicodeExtraField(String text, byte[] bytes) {
            init(text, bytes, 0, bytes.Length);
        }

        private void assembleData() {
            if (unicodeName == null) {
                return;
            }

            data = new byte[5 + unicodeName.Length];
            // version 1
            data[0] = 0x01;
            java.lang.SystemJ.arraycopy(ZipLong.getBytes(nameCRC32), 0, data, 1, 4);
            java.lang.SystemJ.arraycopy(unicodeName, 0, data, 5, unicodeName.Length);
        }

        /**
         * @return The CRC32 checksum of the filename or comment as
         *         encoded in the central directory of the zip file.
         */
        public long getNameCRC32() {
            return nameCRC32;
        }

        /**
         * @param nameCRC32 The CRC32 checksum of the filename as encoded
         *         in the central directory of the zip file to set.
         */
        public void setNameCRC32(long nameCRC32) {
            this.nameCRC32 = nameCRC32;
            data = null;
        }

        /**
         * @return The utf-8 encoded name.
         */
        public byte[] getUnicodeName() {
            return unicodeName;
        }

        /**
         * @param unicodeName The utf-8 encoded name to set.
         */
        public void setUnicodeName(byte[] unicodeName) {
            this.unicodeName = unicodeName;
            data = null;
        }

        /** {@inheritDoc} */
        public byte[] getCentralDirectoryData() {
            if (data == null) {
                this.assembleData();
            }
            return data;
        }

        /** {@inheritDoc} */
        public ZipShort getCentralDirectoryLength() {
            if (data == null) {
                assembleData();
            }
            return new ZipShort(data.Length);
        }

        /** {@inheritDoc} */
        public byte[] getLocalFileDataData() {
            return getCentralDirectoryData();
        }

        /** {@inheritDoc} */
        public ZipShort getLocalFileDataLength() {
            return getCentralDirectoryLength();
        }

        /** {@inheritDoc} */
        public void parseFromLocalFileData(byte[] buffer, int offset, int length)
            //throws ZipException 
            {

            if (length < 5) {
                throw new java.util.zip.ZipException("UniCode path extra data must have at least 5 bytes.");
            }

            int version = buffer[offset];

            if (version != 0x01) {
                throw new java.util.zip.ZipException("Unsupported version [" + version
                                       + "] for UniCode path extra data.");
            }

            nameCRC32 = ZipLong.getValue(buffer, offset + 1);
            unicodeName = new byte[length - 5];
            java.lang.SystemJ.arraycopy(buffer, offset + 5, unicodeName, 0, length - 5);
            data = null;
        }

        /**
         * Doesn't do anything special since this class always uses the
         * same data in central directory and local file data.
         */
        public void parseFromCentralDirectoryData(byte[] buffer, int offset,
                                                  int length)
            //throws ZipException 
            {
            parseFromLocalFileData(buffer, offset, length);
        }
    }
}