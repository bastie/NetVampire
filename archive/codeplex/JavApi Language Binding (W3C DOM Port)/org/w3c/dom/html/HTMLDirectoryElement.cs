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
     * Directory list. See the DIR element definition in HTML 4.0. This element is 
     * deprecated in HTML 4.0.
     */
    public interface HTMLDirectoryElement : HTMLElement
    {
        /**
         * Reduce spacing between list items. See the compact attribute definition 
         * in HTML 4.0. This attribute is deprecated in HTML 4.0.
         */
        bool getCompact();
        void setCompact(bool compact);
    }

}