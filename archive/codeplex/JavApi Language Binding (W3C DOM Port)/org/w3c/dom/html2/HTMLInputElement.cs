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
     * Form control.Depending upon the environment in which the page is being 
     * viewed, the value property may be read-only for the file upload input 
     * type. For the "password" input type, the actual value returned may be 
     * masked to prevent unauthorized use. See the INPUT element definition in [<a href='http://www.w3.org/TR/1999/REC-html401-19991224'>HTML 4.01</a>].
     * <p>See also the <a href='http://www.w3.org/TR/2003/REC-DOM-Level-2-HTML-20030109'>Document Object Model (DOM) Level 2 HTML Specification</a>.
     */
    public interface HTMLInputElement : HTMLElement
    {
        /**
         * When the <code>type</code> attribute of the element has the value 
         * "text", "file" or "password", this represents the HTML value 
         * attribute of the element. The value of this attribute does not change 
         * if the contents of the corresponding form control, in an interactive 
         * user agent, changes. See the value attribute definition in HTML 4.01.
         */
        String getDefaultValue();
        /**
         * When the <code>type</code> attribute of the element has the value 
         * "text", "file" or "password", this represents the HTML value 
         * attribute of the element. The value of this attribute does not change 
         * if the contents of the corresponding form control, in an interactive 
         * user agent, changes. See the value attribute definition in HTML 4.01.
         */
        void setDefaultValue(String defaultValue);

        /**
         * When <code>type</code> has the value "radio" or "checkbox", this 
         * represents the HTML checked attribute of the element. The value of 
         * this attribute does not change if the state of the corresponding form 
         * control, in an interactive user agent, changes. See the checked 
         * attribute definition in HTML 4.01.
         */
        bool getDefaultChecked();
        /**
         * When <code>type</code> has the value "radio" or "checkbox", this 
         * represents the HTML checked attribute of the element. The value of 
         * this attribute does not change if the state of the corresponding form 
         * control, in an interactive user agent, changes. See the checked 
         * attribute definition in HTML 4.01.
         */
        void setDefaultChecked(bool defaultChecked);

        /**
         * Returns the <code>FORM</code> element containing this control. Returns 
         * <code>null</code> if this control is not within the context of a 
         * form. 
         */
        HTMLFormElement getForm();

        /**
         * A comma-separated list of content types that a server processing this 
         * form will handle correctly. See the accept attribute definition in 
         * HTML 4.01.
         */
        String getAccept();
        /**
         * A comma-separated list of content types that a server processing this 
         * form will handle correctly. See the accept attribute definition in 
         * HTML 4.01.
         */
        void setAccept(String accept);

        /**
         * A single character access key to give access to the form control. See 
         * the accesskey attribute definition in HTML 4.01.
         */
        String getAccessKey();
        /**
         * A single character access key to give access to the form control. See 
         * the accesskey attribute definition in HTML 4.01.
         */
        void setAccessKey(String accessKey);

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
         * Alternate text for user agents not rendering the normal content of this 
         * element. See the alt attribute definition in HTML 4.01.
         */
        String getAlt();
        /**
         * Alternate text for user agents not rendering the normal content of this 
         * element. See the alt attribute definition in HTML 4.01.
         */
        void setAlt(String alt);

        /**
         * When the <code>type</code> attribute of the element has the value 
         * "radio" or "checkbox", this represents the current state of the form 
         * control, in an interactive user agent. Changes to this attribute 
         * change the state of the form control, but do not change the value of 
         * the HTML checked attribute of the INPUT element.During the handling 
         * of a click event on an input element with a type attribute that has 
         * the value "radio" or "checkbox", some implementations may change the 
         * value of this property before the event is being dispatched in the 
         * document. If the default action of the event is canceled, the value 
         * of the property may be changed back to its original value. This means 
         * that the value of this property during the handling of click events 
         * is implementation dependent.
         */
        bool getChecked();
        /**
         * When the <code>type</code> attribute of the element has the value 
         * "radio" or "checkbox", this represents the current state of the form 
         * control, in an interactive user agent. Changes to this attribute 
         * change the state of the form control, but do not change the value of 
         * the HTML checked attribute of the INPUT element.During the handling 
         * of a click event on an input element with a type attribute that has 
         * the value "radio" or "checkbox", some implementations may change the 
         * value of this property before the event is being dispatched in the 
         * document. If the default action of the event is canceled, the value 
         * of the property may be changed back to its original value. This means 
         * that the value of this property during the handling of click events 
         * is implementation dependent.
         */
        void setChecked(bool check);

        /**
         * The control is unavailable in this context. See the disabled attribute 
         * definition in HTML 4.01.
         */
        bool getDisabled();
        /**
         * The control is unavailable in this context. See the disabled attribute 
         * definition in HTML 4.01.
         */
        void setDisabled(bool disabled);

        /**
         * Maximum number of characters for text fields, when <code>type</code> 
         * has the value "text" or "password". See the maxlength attribute 
         * definition in HTML 4.01.
         */
        int getMaxLength();
        /**
         * Maximum number of characters for text fields, when <code>type</code> 
         * has the value "text" or "password". See the maxlength attribute 
         * definition in HTML 4.01.
         */
        void setMaxLength(int maxLength);

        /**
         * Form control or object name when submitted with a form. See the name 
         * attribute definition in HTML 4.01.
         */
        String getName();
        /**
         * Form control or object name when submitted with a form. See the name 
         * attribute definition in HTML 4.01.
         */
        void setName(String name);

        /**
         * This control is read-only. Relevant only when <code>type</code> has the 
         * value "text" or "password". See the readonly attribute definition in 
         * HTML 4.01.
         */
        bool getReadOnly();
        /**
         * This control is read-only. Relevant only when <code>type</code> has the 
         * value "text" or "password". See the readonly attribute definition in 
         * HTML 4.01.
         */
        void setReadOnly(bool readOnly);

        /**
         * Size information. The precise meaning is specific to each type of 
         * field. See the size attribute definition in HTML 4.01.
         * @version DOM Level 2
         */
        int getSize();
        /**
         * Size information. The precise meaning is specific to each type of 
         * field. See the size attribute definition in HTML 4.01.
         * @version DOM Level 2
         */
        void setSize(int size);

        /**
         * When the <code>type</code> attribute has the value "image", this 
         * attribute specifies the location of the image to be used to decorate 
         * the graphical submit button. See the src attribute definition in HTML 
         * 4.01.
         */
        String getSrc();
        /**
         * When the <code>type</code> attribute has the value "image", this 
         * attribute specifies the location of the image to be used to decorate 
         * the graphical submit button. See the src attribute definition in HTML 
         * 4.01.
         */
        void setSrc(String src);

        /**
         * Index that represents the element's position in the tabbing order. See 
         * the tabindex attribute definition in HTML 4.01.
         */
        int getTabIndex();
        /**
         * Index that represents the element's position in the tabbing order. See 
         * the tabindex attribute definition in HTML 4.01.
         */
        void setTabIndex(int tabIndex);

        /**
         * The type of control created (all lower case). See the type attribute 
         * definition in HTML 4.01.
         * @version DOM Level 2
         */
        String getType();
        /**
         * The type of control created (all lower case). See the type attribute 
         * definition in HTML 4.01.
         * @version DOM Level 2
         */
        void setType(String type);

        /**
         * Use client-side image map. See the usemap attribute definition in HTML 
         * 4.01.
         */
        String getUseMap();
        /**
         * Use client-side image map. See the usemap attribute definition in HTML 
         * 4.01.
         */
        void setUseMap(String useMap);

        /**
         * When the <code>type</code> attribute of the element has the value 
         * "text", "file" or "password", this represents the current contents of 
         * the corresponding form control, in an interactive user agent. 
         * Changing this attribute changes the contents of the form control, but 
         * does not change the value of the HTML value attribute of the element. 
         * When the <code>type</code> attribute of the element has the value 
         * "button", "hidden", "submit", "reset", "image", "checkbox" or 
         * "radio", this represents the HTML value attribute of the element. See 
         * the value attribute definition in HTML 4.01.
         */
        String getValue();
        /**
         * When the <code>type</code> attribute of the element has the value 
         * "text", "file" or "password", this represents the current contents of 
         * the corresponding form control, in an interactive user agent. 
         * Changing this attribute changes the contents of the form control, but 
         * does not change the value of the HTML value attribute of the element. 
         * When the <code>type</code> attribute of the element has the value 
         * "button", "hidden", "submit", "reset", "image", "checkbox" or 
         * "radio", this represents the HTML value attribute of the element. See 
         * the value attribute definition in HTML 4.01.
         */
        void setValue(String value);

        /**
         * Removes keyboard focus from this element.
         */
        void blur();

        /**
         * Gives keyboard focus to this element.
         */
        void focus();

        /**
         * Select the contents of the text area. For <code>INPUT</code> elements 
         * whose <code>type</code> attribute has one of the following values: 
         * "text", "file", or "password".
         */
        void select();

        /**
         * Simulate a mouse-click. For <code>INPUT</code> elements whose 
         * <code>type</code> attribute has one of the following values: 
         * "button", "checkbox", "radio", "reset", or "submit".
         */
        void click();

    }
}