/*
 * Copyright (c) 2003 World Wide Web Consortium,
 * (Massachusetts Institute of Technology, Institut National de
 * Recherche en Informatique et en Automatique, Keio University). All
 * Rights Reserved. This program is distributed under the W3C's Software
 * Intellectual Property License. This program is distributed in the
 * hope that it will be useful, but WITHOUT ANY WARRANTY; without even
 * the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR
 * PURPOSE.
 * See W3C License http://www.w3.org/Consortium/Legal/ for more details.
 */
using System;

namespace org.w3c.dom.html2
{

    /**
     * Inline subwindows. See the IFRAME element definition in HTML 4.01.
     * <p>See also the <a href='http://www.w3.org/TR/2003/REC-DOM-Level-2-HTML-20030109'>Document Object Model (DOM) Level 2 HTML Specification</a>.
     */
    public interface HTMLIFrameElement : HTMLElement
    {
        /**
         * Aligns this object (vertically or horizontally) with respect to its 
         * surrounding text. See the align attribute definition in HTML 4.01. 
         * This attribute is deprecated in HTML 4.01.
         */
        String getAlign();
        /**
         * Aligns this object (vertically or horizontally) with respect to its 
         * surrounding text. See the align attribute definition in HTML 4.01. 
         * This attribute is deprecated in HTML 4.01.
         */
        void setAlign(String align);

        /**
         * Request frame borders. See the frameborder attribute definition in HTML 
         * 4.01.
         */
        String getFrameBorder();
        /**
         * Request frame borders. See the frameborder attribute definition in HTML 
         * 4.01.
         */
        void setFrameBorder(String frameBorder);

        /**
         * Frame height. See the height attribute definition in HTML 4.01.
         */
        String getHeight();
        /**
         * Frame height. See the height attribute definition in HTML 4.01.
         */
        void setHeight(String height);

        /**
         * URI [<a href='http://www.ietf.org/rfc/rfc2396.txt'>IETF RFC 2396</a>] designating a long description of this image or frame. See the 
         * longdesc attribute definition in HTML 4.01.
         */
        String getLongDesc();
        /**
         * URI [<a href='http://www.ietf.org/rfc/rfc2396.txt'>IETF RFC 2396</a>] designating a long description of this image or frame. See the 
         * longdesc attribute definition in HTML 4.01.
         */
        void setLongDesc(String longDesc);

        /**
         * Frame margin height, in pixels. See the marginheight attribute 
         * definition in HTML 4.01.
         */
        String getMarginHeight();
        /**
         * Frame margin height, in pixels. See the marginheight attribute 
         * definition in HTML 4.01.
         */
        void setMarginHeight(String marginHeight);

        /**
         * Frame margin width, in pixels. See the marginwidth attribute definition 
         * in HTML 4.01.
         */
        String getMarginWidth();
        /**
         * Frame margin width, in pixels. See the marginwidth attribute definition 
         * in HTML 4.01.
         */
        void setMarginWidth(String marginWidth);

        /**
         * The frame name (object of the <code>target</code> attribute). See the 
         * name attribute definition in HTML 4.01.
         */
        String getName();
        /**
         * The frame name (object of the <code>target</code> attribute). See the 
         * name attribute definition in HTML 4.01.
         */
        void setName(String name);

        /**
         * Specify whether or not the frame should have scrollbars. See the 
         * scrolling attribute definition in HTML 4.01.
         */
        String getScrolling();
        /**
         * Specify whether or not the frame should have scrollbars. See the 
         * scrolling attribute definition in HTML 4.01.
         */
        void setScrolling(String scrolling);

        /**
         * A URI [<a href='http://www.ietf.org/rfc/rfc2396.txt'>IETF RFC 2396</a>] designating the initial frame contents. See the src attribute 
         * definition in HTML 4.01.
         */
        String getSrc();
        /**
         * A URI [<a href='http://www.ietf.org/rfc/rfc2396.txt'>IETF RFC 2396</a>] designating the initial frame contents. See the src attribute 
         * definition in HTML 4.01.
         */
        void setSrc(String src);

        /**
         * Frame width. See the width attribute definition in HTML 4.01.
         */
        String getWidth();
        /**
         * Frame width. See the width attribute definition in HTML 4.01.
         */
        void setWidth(String width);

        /**
         * The document this frame contains, if there is any and it is available, 
         * or <code>null</code> otherwise.
         * @since DOM Level 2
         */
        Document getContentDocument();

    }
}