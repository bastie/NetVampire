/*
 *  Licensed under the Apache License, Version 2.0 (the "License");
 *  you may not use this file except in compliance with the License.
 *  You may obtain a copy of the License at 
 * 
 *     http://www.apache.org/licenses/LICENSE-2.0
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

namespace org.apache.commons.collections.buffer
{

    /** 
     * CircularFifoBuffer is a first in first out buffer with a fixed size that
     * replaces its oldest element if full.
     * <p>
     * The removal order of a <code>CircularFifoBuffer</code> is based on the 
     * insertion order; elements are removed in the same order in which they
     * were added.  The iteration order is the same as the removal order.
     * <p>
     * The {@link #add(Object)}, {@link #remove()} and {@link #get()} operations
     * all perform in constant time.  All other operations perform in linear
     * time or worse.
     * <p>
     * Note that this implementation is not synchronized.  The following can be
     * used to provide synchronized access to your <code>CircularFifoBuffer</code>:
     * <pre>
     *   Buffer fifo = BufferUtils.synchronizedBuffer(new CircularFifoBuffer());
     * </pre>
     * <p>
     * This buffer prevents null objects from being added.
     * <p>
     * This class is java.io.Serializable from Commons Collections 3.1.
     * 
     * @since Commons Collections 3.0
     * @version $Revision: 7174 $ $Date: 2011-06-06 22:24:01 +0200 (Mo, 06. Jun 2011) $
     * 
     * @author Stefano Fornari
     * @author Stephen Colebourne
     */
    [Serializable]
    public class CircularFifoBuffer : BoundedFifoBuffer
    {

        /** Serialization version */
        private static readonly long serialVersionUID = -8423413834657610406L;

        /**
         * Constructor that creates a buffer with the default size of 32.
         */
        public CircularFifoBuffer()
            : base(32)
        {
        }

        /**
         * Constructor that creates a buffer with the specified size.
         * 
         * @param size  the size of the buffer (cannot be changed)
         * @throws IllegalArgumentException  if the size is less than 1
         */
        public CircularFifoBuffer(int size)
            : base(size)
        {
        }

        /**
         * Constructor that creates a buffer from the specified collection.
         * The collection size also sets the buffer size
         * 
         * @param coll  the collection to copy into the buffer, may not be null
         * @throws NullPointerException if the collection is null
         */
        public CircularFifoBuffer(java.util.Collection<Object> coll)
            : base(coll)
        {
        }

        /**
         * If the buffer is full, the least recently added element is discarded so
         * that a new element can be inserted.
         *
         * @param element the element to add
         * @return true, always
         */
        public override bool add(Object element)
        {
            if (isFull())
            {
                remove();
            }
            return base.add(element);
        }

    }
}