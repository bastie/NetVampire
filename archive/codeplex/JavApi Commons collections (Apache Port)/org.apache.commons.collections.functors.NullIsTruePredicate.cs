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
     * Predicate implementation that returns true if the input is null.
     * 
     * @since Commons Collections 3.0
     * @version $Revision: 7136 $ $Date: 2011-06-04 21:03:12 +0200 (Sa, 04. Jun 2011) $
     *
     * @author Stephen Colebourne
     */
    [Serializable]
    public sealed class NullIsTruePredicate : Predicate, PredicateDecorator, java.io.Serializable
    {

        /** Serial version UID */
        private static readonly long serialVersionUID = -7625133768987126273L;

        /** The predicate to decorate */
        private readonly Predicate iPredicate;

        /**
         * Factory to create the null true predicate.
         * 
         * @param predicate  the predicate to decorate, not null
         * @return the predicate
         * @throws IllegalArgumentException if the predicate is null
         */
        public static Predicate getInstance(Predicate predicate)
        {
            if (predicate == null)
            {
                throw new java.lang.IllegalArgumentException("Predicate must not be null");
            }
            return new NullIsTruePredicate(predicate);
        }

        /**
         * Constructor that performs no validation.
         * Use <code>getInstance</code> if you want that.
         * 
         * @param predicate  the predicate to call after the null check
         */
        public NullIsTruePredicate(Predicate predicate)
            : base()
        {
            iPredicate = predicate;
        }

        /**
         * Evaluates the predicate returning the result of the decorated predicate
         * once a null check is performed.
         * 
         * @param object  the input object
         * @return true if decorated predicate returns true or input is null
         */
        public bool evaluate(Object obj)
        {
            if (obj == null)
            {
                return true;
            }
            return iPredicate.evaluate(obj);
        }

        /**
         * Gets the predicate being decorated.
         * 
         * @return the predicate as the only element in an array
         * @since Commons Collections 3.1
         */
        public Predicate[] getPredicates()
        {
            return new Predicate[] { iPredicate };
        }

    }
}