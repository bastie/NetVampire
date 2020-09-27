/*
 * Copyright (c) 2004 World Wide Web Consortium,
 *
 * (Massachusetts Institute of Technology, European Research Consortium for
 * Informatics and Mathematics, Keio University). All Rights Reserved. This
 * work is distributed under the W3C(r) Software License [1] in the hope that
 * it will be useful, but WITHOUT ANY WARRANTY; without even the implied
 * warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
 *
 * [1] http://www.w3.org/Consortium/Legal/2002/copyright-software-20021231
 */
using org.w3c.dom.events;

namespace org.w3c.dom.ls
{

    /**
     *  This interface represents a load event object that signals the completion 
     * of a document load. 
     * <p>See also the <a href='http://www.w3.org/TR/2004/REC-DOM-Level-3-LS-20040407'>Document Object Model (DOM) Level 3 Load
    and Save Specification</a>.
     */
    public interface LSLoadEvent : Event
    {
        /**
         * The document that finished loading.
         */
        Document getNewDocument();

        /**
         * The input source that was parsed.
         */
        LSInput getInput();

    }
}