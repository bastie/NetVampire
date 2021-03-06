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
using org.apache.commons.collections;

namespace org.apache.commons.collections.iterators
{

    /** 
     * Implements an {@link java.util.Iterator Iterator} over any array.
     * <p>
     * The array can be either an array of object or of primitives. If you know 
     * that you have an object array, the 
     * {@link org.apache.commons.collections.iterators.ObjectArrayIterator ObjectArrayIterator}
     * class is a better choice, as it will perform better.
     * <p>
     * The iterator implements a {@link #reset} method, allowing the reset of 
     * the iterator back to the start if required.
     *
     * @since Commons Collections 1.0
     * @version $Revision: 7263 $ $Date: 2011-06-12 13:28:27 +0200 (So, 12. Jun 2011) $
     *
     * @author James Strachan
     * @author Mauricio S. Moura
     * @author Michael A. Smith
     * @author Neil O'Toole
     * @author Stephen Colebourne
     */
    public class ArrayIterator : ResettableIterator
    {

        /** The array to iterate over */
        protected Object array;
        /** The start index to loop from */
        protected int startIndex = 0;
        /** The end index to loop to */
        protected int endIndex = 0;
        /** The current iterator index */
        protected int index = 0;

        // Constructors
        // ----------------------------------------------------------------------
        /**
         * Constructor for use with <code>setArray</code>.
         * <p>
         * Using this constructor, the iterator is equivalent to an empty iterator
         * until {@link #setArray(Object)} is  called to establish the array to iterate over.
         */
        public ArrayIterator() :base()
        {
        }

        /**
         * Constructs an ArrayIterator that will iterate over the values in the
         * specified array.
         *
         * @param array the array to iterate over.
         * @throws IllegalArgumentException if <code>array</code> is not an array.
         * @throws NullPointerException if <code>array</code> is <code>null</code>
         */
        public ArrayIterator(Object array)
            : base()
        {
            setArray(array);
        }

        /**
         * Constructs an ArrayIterator that will iterate over the values in the
         * specified array from a specific start index.
         *
         * @param array  the array to iterate over.
         * @param startIndex  the index to start iterating at.
         * @throws IllegalArgumentException if <code>array</code> is not an array.
         * @throws NullPointerException if <code>array</code> is <code>null</code>
         * @throws IndexOutOfBoundsException if the index is invalid
         */
        public ArrayIterator(Object array, int startIndex)
            : base()
        {
            setArray(array);
            checkBound(startIndex, "start");
            this.startIndex = startIndex;
            this.index = startIndex;
        }

        /**
         * Construct an ArrayIterator that will iterate over a range of values 
         * in the specified array.
         *
         * @param array  the array to iterate over.
         * @param startIndex  the index to start iterating at.
         * @param endIndex  the index to finish iterating at.
         * @throws IllegalArgumentException if <code>array</code> is not an array.
         * @throws NullPointerException if <code>array</code> is <code>null</code>
         * @throws IndexOutOfBoundsException if either index is invalid
         */
        public ArrayIterator(Object array, int startIndex, int endIndex)
            : base()
        {
            setArray(array);
            checkBound(startIndex, "start");
            checkBound(endIndex, "end");
            if (endIndex < startIndex)
            {
                throw new java.lang.IllegalArgumentException("End index must not be less than start index.");
            }
            this.startIndex = startIndex;
            this.endIndex = endIndex;
            this.index = startIndex;
        }

        /**
         * Checks whether the index is valid or not.
         * 
         * @param bound  the index to check
         * @param type  the index type (for error messages)
         * @throws IndexOutOfBoundsException if the index is invalid
         */
        protected virtual void checkBound(int bound, String type)
        {
            if (bound > this.endIndex)
            {
                throw new java.lang.ArrayIndexOutOfBoundsException(
                  "Attempt to make an ArrayIterator that " + type +
                  "s beyond the end of the array. "
                );
            }
            if (bound < 0)
            {
                throw new java.lang.ArrayIndexOutOfBoundsException(
                  "Attempt to make an ArrayIterator that " + type +
                  "s before the start of the array. "
                );
            }
        }

        // Iterator interface
        //-----------------------------------------------------------------------
        /**
         * Returns true if there are more elements to return from the array.
         *
         * @return true if there is a next element to return
         */
        public virtual bool hasNext()
        {
            return (index < endIndex);
        }

        /**
         * Returns the next element in the array.
         *
         * @return the next element in the array
         * @throws NoSuchElementException if all the elements in the array
         *  have already been returned
         */
        public virtual Object next()
        {
            if (hasNext() == false)
            {
                throw new java.util.NoSuchElementException();
            }
            return java.lang.reflect.Array.get(array, index++);
        }

        /**
         * Throws {@link UnsupportedOperationException}.
         *
         * @throws UnsupportedOperationException always
         */
        public virtual void remove()
        {
            throw new java.lang.UnsupportedOperationException("remove() method is not supported");
        }

        // Properties
        //-----------------------------------------------------------------------
        /**
         * Gets the array that this iterator is iterating over. 
         *
         * @return the array this iterator iterates over, or <code>null</code> if
         *  the no-arg constructor was used and {@link #setArray(Object)} has never
         *  been called with a valid array.
         */
        public virtual Object getArray()
        {
            return array;
        }

        /**
         * Sets the array that the ArrayIterator should iterate over.
         * <p>
         * If an array has previously been set (using the single-arg constructor
         * or this method) then that array is discarded in favour of this one.
         * Iteration is restarted at the start of the new array.
         * Although this can be used to reset iteration, the {@link #reset()} method
         * is a more effective choice.
         *
         * @param array the array that the iterator should iterate over.
         * @throws IllegalArgumentException if <code>array</code> is not an array.
         * @throws NullPointerException if <code>array</code> is <code>null</code>
         */
        public virtual void setArray(Object array)
        {
            // Array.getLength throws IllegalArgumentException if the object is not
            // an array or NullPointerException if the object is null.  This call
            // is made before saving the array and resetting the index so that the
            // array iterator remains in a consistent state if the argument is not
            // an array or is null.
            this.endIndex = java.lang.reflect.Array.getLength(array);
            this.startIndex = 0;
            this.array = array;
            this.index = 0;
        }

        /**
         * Resets the iterator back to the start index.
         */
        public virtual void reset()
        {
            this.index = this.startIndex;
        }

    }
}