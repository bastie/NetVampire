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

namespace org.apache.commons.collections.functors
{

    /**
     * Transformer implementation that returns the same constant each time.
     * <p>
     * No check is made that the object is immutable. In general, only immutable
     * objects should use the constant factory. Mutable objects should
     * use the prototype factory.
     * 
     * @since Commons Collections 3.0
     * @version $Revision: 7136 $ $Date: 2011-06-04 21:03:12 +0200 (Sa, 04. Jun 2011) $
     *
     * @author Stephen Colebourne
     */
    [Serializable]
    public class ConstantTransformer : Transformer, java.io.Serializable
    {

        /** Serial version UID */
        private static readonly long serialVersionUID = 6374440726369055124L;

        /** Returns null each time */
        public static readonly Transformer NULL_INSTANCE = new ConstantTransformer(null);

        /** The closures to call in turn */
        private readonly Object iConstant;

        /**
         * Transformer method that performs validation.
         *
         * @param constantToReturn  the constant object to return each time in the factory
         * @return the <code>constant</code> factory.
         */
        public static Transformer getInstance(Object constantToReturn)
        {
            if (constantToReturn == null)
            {
                return NULL_INSTANCE;
            }
            return new ConstantTransformer(constantToReturn);
        }

        /**
         * Constructor that performs no validation.
         * Use <code>getInstance</code> if you want that.
         * 
         * @param constantToReturn  the constant to return each time
         */
        public ConstantTransformer(Object constantToReturn)
            : base()
        {
            iConstant = constantToReturn;
        }

        /**
         * Transforms the input by ignoring it and returning the stored constant instead.
         * 
         * @param input  the input object which is ignored
         * @return the stored constant
         */
        public Object transform(Object input)
        {
            return iConstant;
        }

        /**
         * Gets the constant.
         * 
         * @return the constant
         * @since Commons Collections 3.1
         */
        public Object getConstant()
        {
            return iConstant;
        }

    }
}