/*
 * Copyright (c) 1999 World Wide Web Consortium,
 * (Massachusetts Institute of Technology, Institut National de Recherche
 *  en Informatique et en Automatique, Keio University).
 * All Rights Reserved. http://www.w3.org/Consortium/Legal/
 */
using System;

namespace org.w3c.dom.html
{

    /**
     * Document head information. See the HEAD element definition in HTML 4.0.
     */
    public interface HTMLHeadElement : HTMLElement
    {
        /**
         * URI designating a metadata profile. See the profile attribute definition 
         * in HTML 4.0.
         */
        String getProfile();
        void setProfile(String profile);
    }

}