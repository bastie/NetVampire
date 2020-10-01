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

namespace org.apache.commons.compress.archivers.zip{

    /// <summary>
    /// Utility class that represents a four byte integer with conversion
    /// rules for the big endian byte order of ZIP files. 
    /// @Immutable
    /// </summary>
    public sealed class ZipLong : java.lang.Cloneable {

        private static readonly int WORD = 4;
        //private static readonly int BYTE_BIT_SIZE = 8;
        private static readonly int BYTE_MASK = 0xFF;

        private static readonly int BYTE_1 = 1;
        private static readonly int BYTE_1_MASK = 0xFF00;
        private static readonly int BYTE_1_SHIFT = 8;

        private static readonly int BYTE_2 = 2;
        private static readonly int BYTE_2_MASK = 0xFF0000;
        private static readonly int BYTE_2_SHIFT = 16;

        private static readonly int BYTE_3 = 3;
        private static readonly long BYTE_3_MASK = 0xFF000000L;
        private static readonly int BYTE_3_SHIFT = 24;

        private readonly long value;

        /** Central File Header Signature */
        public static readonly ZipLong CFH_SIG = new ZipLong(0X02014B50L);

        /** Local File Header Signature */
        public static readonly ZipLong LFH_SIG = new ZipLong(0X04034B50L);

        /**
         * Data Descriptor signature
         * @since Apache Commons Compress 1.1
         */
        public static readonly ZipLong DD_SIG = new ZipLong(0X08074B50L);

        /**
         * Create instance from a number.
         * @param value the long to store as a ZipLong
         */
        public ZipLong(long value) {
            this.value = value;
        }

        /**
         * Create instance from bytes.
         * @param bytes the bytes to store as a ZipLong
         */
        public ZipLong (byte[] bytes) {
            value = ZipLong.getValue(bytes, 0);
        }

        /**
         * Create instance from the four bytes starting at offset.
         * @param bytes the bytes to store as a ZipLong
         * @param offset the offset to start
         */
        public ZipLong (byte[] bytes, int offset) {
            value = ZipLong.getValue(bytes, offset);
        }

        /**
         * Get value as four bytes in big endian byte order.
         * @return value as four bytes in big endian order
         */
        public byte[] getBytes() {
            return ZipLong.getBytes(value);
        }

        /**
         * Get value as Java long.
         * @return value as a long
         */
        public long getValue() {
            return value;
        }

        /**
         * Get value as four bytes in big endian byte order.
         * @param value the value to convert
         * @return value as four bytes in big endian byte order
         */
        public static byte[] getBytes(long value) {
            byte[] result = new byte[WORD];
            result[0] = (byte) ((value & BYTE_MASK));
            result[BYTE_1] = (byte) ((value & BYTE_1_MASK) >> BYTE_1_SHIFT);
            result[BYTE_2] = (byte) ((value & BYTE_2_MASK) >> BYTE_2_SHIFT);
            result[BYTE_3] = (byte) ((value & BYTE_3_MASK) >> BYTE_3_SHIFT);
            return result;
        }

        /**
         * Helper method to get the value as a Java long from four bytes starting at given array offset
         * @param bytes the array of bytes
         * @param offset the offset to start
         * @return the correspondanding Java long value
         */
        public static long getValue(byte[] bytes, int offset) {
            long value = (bytes[offset + BYTE_3] << BYTE_3_SHIFT) & BYTE_3_MASK;
            value += (bytes[offset + BYTE_2] << BYTE_2_SHIFT) & BYTE_2_MASK;
            value += (bytes[offset + BYTE_1] << BYTE_1_SHIFT) & BYTE_1_MASK;
            value += (bytes[offset] & BYTE_MASK);
            return value;
        }

        /**
         * Helper method to get the value as a Java long from a four-byte array
         * @param bytes the array of bytes
         * @return the correspondanding Java long value
         */
        public static long getValue(byte[] bytes) {
            return getValue(bytes, 0);
        }

        /**
         * Override to make two instances with same value equal.
         * @param o an object to compare
         * @return true if the objects are equal
         */
        public override bool Equals(Object o) {
            if (o == null || !(o is ZipLong)) {
                return false;
            }
            return value == ((ZipLong) o).getValue();
        }

        /**
         * Override to make two instances with same value equal.
         * @return the value stored in the ZipLong
         */
        public override int GetHashCode() {
            return (int) value;
        }

        public Object clone() {
            try {
                return base.MemberwiseClone();
            } catch (Exception cnfe) {
                // impossible
                throw new java.lang.RuntimeException(new java.lang.CloneNotSupportedException (cnfe.Message));
            }
        }
    }
}