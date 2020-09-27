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
     * Table caption See the CAPTION element definition in HTML 4.0.
     */
    public interface HTMLTableCaptionElement : HTMLElement
    {
        /**
         * Caption alignment with respect to the table. See the align attribute 
         * definition in HTML 4.0. This attribute is deprecated in HTML 4.0.
         */
        String getAlign();
        void setAlign(String align);
    }

}