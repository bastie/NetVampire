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

namespace org.apache.commons.collections
{

    /**
     * Defines a functor interface implemented by classes that transform one
     * object into another.
     * <p>
     * A <code>Transformer</code> converts the input object to the output object.
     * The input object should be left unchanged.
     * Transformers are typically used for type conversions, or extracting data
     * from an object.
     * <p>
     * Standard implementations of common transformers are provided by
     * {@link TransformerUtils}. These include method invokation, returning a constant,
     * cloning and returning the string value.
     * 
     * @since Commons Collections 1.0
     * @version $Revision: 7130 $ $Date: 2011-06-04 20:48:14 +0200 (Sa, 04. Jun 2011) $
     * 
     * @author James Strachan
     * @author Stephen Colebourne
     */
    public interface Transformer
    {

        /**
         * Transforms the input object (leaving it unchanged) into some output object.
         *
         * @param input  the object to be transformed, should be left unchanged
         * @return a transformed object
         * @throws ClassCastException (runtime) if the input is the wrong class
         * @throws IllegalArgumentException (runtime) if the input is invalid
         * @throws FunctorException (runtime) if the transform cannot be completed
         */
        Object transform(Object input);

    }
}