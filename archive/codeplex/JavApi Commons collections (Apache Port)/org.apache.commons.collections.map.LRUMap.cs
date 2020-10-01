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

namespace org.apache.commons.collections.map
{

    /**
     * A <code>Map</code> implementation with a fixed maximum size which removes
     * the least recently used entry if an entry is added when full.
     * <p>
     * The least recently used algorithm works on the get and put operations only.
     * Iteration of any kind, including setting the value by iteration, does not
     * change the order. Queries such as containsKey and containsValue or access
     * via views also do not change the order.
     * <p>
     * The map implements <code>OrderedMap</code> and entries may be queried using
     * the bidirectional <code>OrderedMapIterator</code>. The order returned is
     * least recently used to most recently used. Iterators from map views can 
     * also be cast to <code>OrderedIterator</code> if required.
     * <p>
     * All the available iterators can be reset back to the start by casting to
     * <code>ResettableIterator</code> and calling <code>reset()</code>.
     * <p>
     * <strong>Note that LRUMap is not synchronized and is not thread-safe.</strong>
     * If you wish to use this map from multiple threads concurrently, you must use
     * appropriate synchronization. The simplest approach is to wrap this map
     * using {@link java.util.Collections#synchronizedMap(Map)}. This class may throw 
     * <code>NullPointerException</code>'s when accessed by concurrent threads.
     *
     * @since Commons Collections 3.0 (previously in main package v1.0)
     * @version $Revision: 7267 $ $Date: 2011-06-13 12:58:24 +0200 (Mo, 13. Jun 2011) $
     *
     * @author James Strachan
     * @author Morgan Delagrange
     * @author Stephen Colebourne
     * @author Mike Pettypiece
     * @author Mario Ivankovits
     */
    [Serializable]
    public class LRUMap
            : AbstractLinkedMap, BoundedMap, java.io.Serializable, java.lang.Cloneable
    {

        /** Serialisation version */
        private static readonly long serialVersionUID = -612114643488955218L;
        /** Default maximum size */
        protected static readonly int DEFAULT_MAX_SIZE = 100;

        /** Maximum size */
        [NonSerialized]
        private int maxSizeJ;
        /** Scan behaviour */
        private bool scanUntilRemovable;

        /**
         * Constructs a new empty map with a maximum size of 100.
         */
        public LRUMap() :
            this(DEFAULT_MAX_SIZE, DEFAULT_LOAD_FACTOR, false)
        {
        }

        /**
         * Constructs a new, empty map with the specified maximum size.
         *
         * @param maxSize  the maximum size of the map
         * @throws IllegalArgumentException if the maximum size is less than one
         */
        public LRUMap(int maxSize) :
            this(maxSize, DEFAULT_LOAD_FACTOR)
        {
        }

        /**
         * Constructs a new, empty map with the specified maximum size.
         *
         * @param maxSize  the maximum size of the map
         * @param scanUntilRemovable  scan until a removeable entry is found, default false
         * @throws IllegalArgumentException if the maximum size is less than one
         * @since Commons Collections 3.1
         */
        public LRUMap(int maxSize, bool scanUntilRemovable) :
            this(maxSize, DEFAULT_LOAD_FACTOR, scanUntilRemovable)
        {
        }

        /**
         * Constructs a new, empty map with the specified initial capacity and
         * load factor. 
         *
         * @param maxSize  the maximum size of the map, -1 for no limit,
         * @param loadFactor  the load factor
         * @throws IllegalArgumentException if the maximum size is less than one
         * @throws IllegalArgumentException if the load factor is less than zero
         */
        public LRUMap(int maxSize, float loadFactor) :
            this(maxSize, loadFactor, false)
        {
        }

        /**
         * Constructs a new, empty map with the specified initial capacity and
         * load factor.
         *
         * @param maxSize  the maximum size of the map, -1 for no limit,
         * @param loadFactor  the load factor
         * @param scanUntilRemovable  scan until a removeable entry is found, default false
         * @throws IllegalArgumentException if the maximum size is less than one
         * @throws IllegalArgumentException if the load factor is less than zero
         * @since Commons Collections 3.1
         */
        public LRUMap(int maxSize, float loadFactor, bool scanUntilRemovable) :
            base((maxSize < 1 ? DEFAULT_CAPACITY : maxSize), loadFactor)
        {
            if (maxSize < 1)
            {
                throw new java.lang.IllegalArgumentException("LRUMap max size must be greater than 0");
            }
            this.maxSizeJ = maxSize;
            this.scanUntilRemovable = scanUntilRemovable;
        }

        /**
         * Constructor copying elements from another map.
         * <p>
         * The maximum size is set from the map's size.
         *
         * @param map  the map to copy
         * @throws NullPointerException if the map is null
         * @throws IllegalArgumentException if the map is empty
         */
        public LRUMap(java.util.Map<Object, Object> map)
            : this(map, false)
        {
        }

        /**
         * Constructor copying elements from another map.
         * <p/>
         * The maximum size is set from the map's size.
         *
         * @param map  the map to copy
         * @param scanUntilRemovable  scan until a removeable entry is found, default false
         * @throws NullPointerException if the map is null
         * @throws IllegalArgumentException if the map is empty
         * @since Commons Collections 3.1
         */
        public LRUMap(java.util.Map<Object, Object> map, bool scanUntilRemovable) :
            this(map.size(), DEFAULT_LOAD_FACTOR, scanUntilRemovable)
        {
            putAll(map);
        }

        //-----------------------------------------------------------------------
        /**
         * Gets the value mapped to the key specified.
         * <p>
         * This operation changes the position of the key in the map to the
         * most recently used position (first).
         * 
         * @param key  the key
         * @return the mapped value, null if no match
         */
        public override Object get(Object key)
        {
            LinkEntry entry = (LinkEntry)getEntry(key);
            if (entry == null)
            {
                return null;
            }
            moveToMRU(entry);
            return entry.getValue();
        }

        //-----------------------------------------------------------------------
        /**
         * Moves an entry to the MRU position at the end of the list.
         * <p>
         * This implementation moves the updated entry to the end of the list.
         * 
         * @param entry  the entry to update
         */
        protected virtual void moveToMRU(LinkEntry entry)
        {
            if (entry.after != header)
            {
                modCount++;
                // remove
                entry.before.after = entry.after;
                entry.after.before = entry.before;
                // add first
                entry.after = header;
                entry.before = header.before;
                header.before.after = entry;
                header.before = entry;
            }
            else if (entry == header)
            {
                throw new java.lang.IllegalStateException("Can't move header to MRU" +
                    " (please report this to commons-dev@jakarta.apache.org)");
            }
        }

        /**
         * Updates an existing key-value mapping.
         * <p>
         * This implementation moves the updated entry to the top of the list
         * using {@link #moveToMRU(AbstractLinkedMap.LinkEntry)}.
         * 
         * @param entry  the entry to update
         * @param newValue  the new value to store
         */
        protected internal override void updateEntry(HashEntry entry, Object newValue)
        {
            moveToMRU((LinkEntry)entry);  // handles modCount
            entry.setValue(newValue);
        }

        /**
         * Adds a new key-value mapping into this map.
         * <p>
         * This implementation checks the LRU size and determines whether to
         * discard an entry or not using {@link #removeLRU(AbstractLinkedMap.LinkEntry)}.
         * <p>
         * From Commons Collections 3.1 this method uses {@link #isFull()} rather
         * than accessing <code>size</code> and <code>maxSize</code> directly.
         * It also handles the scanUntilRemovable functionality.
         * 
         * @param hashIndex  the index into the data array to store at
         * @param hashCode  the hash code of the key to add
         * @param key  the key to add
         * @param value  the value to add
         */
        protected internal override void addMapping(int hashIndex, int hashCode, Object key, Object value)
        {
            if (isFull())
            {
                LinkEntry reuse = header.after;
                bool removeLRUEntry = false;
                if (scanUntilRemovable)
                {
                    while (reuse != header && reuse != null)
                    {
                        if (removeLRU(reuse))
                        {
                            removeLRUEntry = true;
                            break;
                        }
                        reuse = reuse.after;
                    }
                    if (reuse == null)
                    {
                        throw new java.lang.IllegalStateException(
                            "Entry.after=null, header.after" + header.after + " header.before" + header.before +
                            " key=" + key + " value=" + value + " size=" + sizeJ + " maxSize=" + maxSizeJ +
                            " Please check that your keys are immutable, and that you have used synchronization properly." +
                            " If so, then please report this to commons-dev@jakarta.apache.org as a bug.");
                    }
                }
                else
                {
                    removeLRUEntry = removeLRU(reuse);
                }

                if (removeLRUEntry)
                {
                    if (reuse == null)
                    {
                        throw new java.lang.IllegalStateException(
                            "reuse=null, header.after=" + header.after + " header.before" + header.before +
                            " key=" + key + " value=" + value + " size=" + sizeJ + " maxSize=" + maxSizeJ +
                            " Please check that your keys are immutable, and that you have used synchronization properly." +
                            " If so, then please report this to commons-dev@jakarta.apache.org as a bug.");
                    }
                    reuseMapping(reuse, hashIndex, hashCode, key, value);
                }
                else
                {
                    base.addMapping(hashIndex, hashCode, key, value);
                }
            }
            else
            {
                base.addMapping(hashIndex, hashCode, key, value);
            }
        }

        /**
         * Reuses an entry by removing it and moving it to a new place in the map.
         * <p>
         * This method uses {@link #removeEntry}, {@link #reuseEntry} and {@link #addEntry}.
         * 
         * @param entry  the entry to reuse
         * @param hashIndex  the index into the data array to store at
         * @param hashCode  the hash code of the key to add
         * @param key  the key to add
         * @param value  the value to add
         */
        protected void reuseMapping(LinkEntry entry, int hashIdx, int hashCode, Object key, Object value)
        {
            // find the entry before the entry specified in the hash table
            // remember that the parameters (except the first) refer to the new entry,
            // not the old one
            try
            {
                int removeIndex = hashIndex(entry.hashCode, data.Length);
                HashEntry[] tmp = data;  // may protect against some sync issues
                HashEntry loop = tmp[removeIndex];
                HashEntry previous = null;
                while (loop != entry && loop != null)
                {
                    previous = loop;
                    loop = loop.nextJ;
                }
                if (loop == null)
                {
                    throw new java.lang.IllegalStateException(
                        "Entry.next=null, data[removeIndex]=" + data[removeIndex] + " previous=" + previous +
                        " key=" + key + " value=" + value + " size=" + sizeJ + " maxSize=" + maxSizeJ +
                        " Please check that your keys are immutable, and that you have used synchronization properly." +
                        " If so, then please report this to commons-dev@jakarta.apache.org as a bug.");
                }

                // reuse the entry
                modCount++;
                removeEntry(entry, removeIndex, previous);
                reuseEntry(entry, hashIdx, hashCode, key, value);
                addEntry(entry, hashIdx);
            }
            catch (java.lang.NullPointerException )
            {
                throw new java.lang.IllegalStateException(
                        "NPE, entry=" + entry + " entryIsHeader=" + (entry == header) +
                        " key=" + key + " value=" + value + " size=" + sizeJ + " maxSize=" + maxSizeJ +
                        " Please check that your keys are immutable, and that you have used synchronization properly." +
                        " If so, then please report this to commons-dev@jakarta.apache.org as a bug.");
            }
        }

        /**
         * Subclass method to control removal of the least recently used entry from the map.
         * <p>
         * This method exists for subclasses to override. A subclass may wish to
         * provide cleanup of resources when an entry is removed. For example:
         * <pre>
         * protected bool removeLRU(LinkEntry entry) {
         *   releaseResources(entry.getValue());  // release resources held by entry
         *   return true;  // actually delete entry
         * }
         * </pre>
         * <p>
         * Alternatively, a subclass may choose to not remove the entry or selectively
         * keep certain LRU entries. For example:
         * <pre>
         * protected bool removeLRU(LinkEntry entry) {
         *   if (entry.getKey().toString().startsWith("System.")) {
         *     return false;  // entry not removed from LRUMap
         *   } else {
         *     return true;  // actually delete entry
         *   }
         * }
         * </pre>
         * The effect of returning false is dependent on the scanUntilRemovable flag.
         * If the flag is true, the next LRU entry will be passed to this method and so on
         * until one returns false and is removed, or every entry in the map has been passed.
         * If the scanUntilRemovable flag is false, the map will exceed the maximum size.
         * <p>
         * NOTE: Commons Collections 3.0 passed the wrong entry to this method.
         * This is fixed in version 3.1 onwards.
         * 
         * @param entry  the entry to be removed
         */
        protected virtual bool removeLRU(LinkEntry entry)
        {
            return true;
        }

        //-----------------------------------------------------------------------
        /**
         * Returns true if this map is full and no new mappings can be added.
         *
         * @return <code>true</code> if the map is full
         */
        public virtual bool isFull()
        {
            return (sizeJ >= maxSizeJ);
        }

        /**
         * Gets the maximum size of the map (the bound).
         *
         * @return the maximum number of elements the map can hold
         */
        public virtual int maxSize()
        {
            return maxSizeJ;
        }

        /**
         * Whether this LRUMap will scan until a removable entry is found when the
         * map is full.
         *
         * @return true if this map scans
         * @since Commons Collections 3.1
         */
        public virtual bool isScanUntilRemovable()
        {
            return scanUntilRemovable;
        }

        //-----------------------------------------------------------------------
        /**
         * Clones the map without cloning the keys or values.
         *
         * @return a shallow clone
         */
        protected override Object clone()
        {
            return base.MemberwiseClone();
        }
        Object java.lang.Cloneable.clone()
        {
            return this.clone();
        }

        /**
         * Write the map out using a custom routine.
         */
        private void writeObject(java.io.ObjectOutputStream outJ)
        {//throws IOException {
            outJ.defaultWriteObject();
            doWriteObject(outJ);
        }

        /**
         * Read the map in using a custom routine.
         */
        private void readObject(java.io.ObjectInputStream inJ)
        {//throws IOException, ClassNotFoundException {
            inJ.defaultReadObject();
            doReadObject(inJ);
        }

        /**
         * Writes the data necessary for <code>put()</code> to work in deserialization.
         */
        protected virtual void dowriteObject(java.io.ObjectOutputStream outJ)
        {//throws IOException {
            outJ.writeInt(maxSizeJ);
            base.doWriteObject(outJ);
        }

        /**
         * Reads the data necessary for <code>put()</code> to work in the superclass.
         */
        protected virtual void doreadObject(java.io.ObjectInputStream inJ)
        {//throws IOException, ClassNotFoundException {
            maxSizeJ = inJ.readInt();
            base.doReadObject(inJ);
        }

    }
}