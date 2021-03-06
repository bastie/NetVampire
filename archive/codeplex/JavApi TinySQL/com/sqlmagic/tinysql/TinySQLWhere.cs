/*
 * tinySQLWhere - Class to handle all Where clause processing.
 * 
 * $Author: davis $
 * $Date: 2004/12/18 21:24:13 $
 * $Revision: 1.1 $
 *
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later version.
 *
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public
 * License along with this library; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307 USA
 *
 * Revision History:
 *
 * Written by Davis Swan in May, 2004.
 */
using System;
using java = biz.ritter.javapi;

namespace com.sqlmagic.tinysql
{

    public class TinySQLWhere
    {
        java.util.Vector<Object> whereClauseList;
        /*
         * The constructor builds a Where clause object from the input string.
         */
        public TinySQLWhere(String whereString, java.util.Hashtable<Object, Object> tableDefs)
        //throws tinySQLException
        {
            FieldTokenizer ft;
            java.util.Vector<Object> whereConditions;
            TsColumn leftColumn, rightColumn;
            Object whereObj;
            java.lang.StringBuffer fieldBuffer;
            String nextField, upperField, wherePhrase, comp, left, right, andOr, lastWord;
            java.util.Vector<Object> whereCondition;
            String[] comparisons = {"<=","=<",">=","=>","=","<>","!=",">","<",
      "LIKE","NOT LIKE","IS"};
            String[] fields, keepFields;
            bool inBrackets = false, foundFunction = false;
            int i, j, foundKeyWord, foundComp, startAt, foundAnd, foundOr, keepCount;
            /*
             *    The whereClauseList is a Vector containing pointers to whereCondition
             *    Vectors or tinySQLWhere objects.
             */
            whereConditions = new java.util.Vector<Object>();
            whereClauseList = new java.util.Vector<Object>();
            /*
             *    Identify any phrases that are contained within brackets.  Note that 
             *    the FieldTokenizer will catch function definitions as well as 
             *    subPhrases so there has to be additional logic to reconstruct 
             *    the functions.
             */
            ft = new FieldTokenizer(whereString, '(', true);
            fields = ft.getFields();
            keepFields = new String[fields.Length];
            lastWord = "NULL";
            fieldBuffer = new java.lang.StringBuffer();
            foundFunction = false;
            keepCount = 0;
            for (i = 0; i < fields.Length; i++)
            {
                keepFields[i] = "";
                if (fields[i].equals("("))
                {
                    /*
                     *          If this is a known function reconstruct the function definition
                     *          and save the entire string.
                     */
                    foundFunction = Utils.isFunctionName(lastWord);
                    if (foundFunction)
                    {
                        fieldBuffer.append("(");
                    }
                    else
                    {
                        if (fieldBuffer.length() > 0)
                        {

                            keepFields[keepCount] = fieldBuffer.toString();
                            keepCount++;
                            fieldBuffer.delete(0, fieldBuffer.length());
                        }
                        keepFields[keepCount] = "(";
                        keepCount++;
                    }
                }
                else if (fields[i].equals(")"))
                {
                    if (foundFunction)
                    {
                        fieldBuffer.append(") ");
                        foundFunction = false;
                    }
                    else
                    {
                        if (fieldBuffer.length() > 0)
                        {
                            keepFields[keepCount] = fieldBuffer.toString();
                            keepCount++;
                            fieldBuffer.delete(0, fieldBuffer.length());
                        }
                        keepFields[keepCount] = ")";
                        keepCount++;
                    }
                }
                else
                {
                    fieldBuffer.append(fields[i]);
                }
                lastWord = fields[i].substring(fields[i].lastIndexOf(" ") + 1);
            }
            /*
             *    Keep last subPhrase
             */
            if (fieldBuffer.length() > 0)
            {
                keepFields[keepCount] = fieldBuffer.toString();
                keepCount++;
            }
            for (i = 0; i < keepCount; i++)
            {
                if (TinySQLGlobals.WHERE_DEBUG)
                    java.lang.SystemJ.outJ.println("keepFields[" + i + "]=" + keepFields[i]);
                nextField = keepFields[i];
                upperField = nextField.toUpperCase();
                if (nextField.equals("("))
                {
                    whereObj = (Object)null;
                    inBrackets = true;
                }
                else if (nextField.equals(")"))
                {
                    inBrackets = false;
                    whereObj = (Object)null;
                }
                else if (inBrackets)
                {
                    whereObj = new TinySQLWhere(nextField, tableDefs);
                    whereConditions.addElement(whereObj);
                }
                else
                {
                    /*
                     *          Look for AND/OR keywords - if none are found process the
                     *          entire string.
                     */
                    andOr = "AND";
                    startAt = 0;
                    while (startAt < upperField.length())
                    {
                        if (upperField.startsWith("AND "))
                        {
                            foundAnd = 0;
                        }
                        else
                        {
                            foundAnd = upperField.indexOf(" AND", startAt);
                            /*
                             *                Make sure this is not just part of a longer string.
                             */
                            if (foundAnd > -1 & foundAnd < upperField.length() - 4)
                                if (upperField.charAt(foundAnd + 4) != ' ')
                                    foundAnd = -1;
                        }
                        if (upperField.startsWith("OR "))
                        {
                            foundOr = 0;
                        }
                        else
                        {
                            foundOr = upperField.indexOf(" OR", startAt);
                            if (foundOr > -1 & foundOr < upperField.length() - 3)
                                if (upperField.charAt(foundOr + 3) != ' ')
                                    foundOr = -1;
                        }
                        foundKeyWord = upperField.length();
                        if (foundAnd > -1) foundKeyWord = foundAnd;
                        if (foundOr > -1 & foundOr < foundKeyWord)
                        {
                            foundKeyWord = foundOr;
                            andOr = "OR";
                        }
                        if (foundKeyWord == 0)
                        {
                            startAt = andOr.length() + 1;
                            foundKeyWord = upperField.length();
                        }
                        wherePhrase = nextField.substring(startAt, foundKeyWord);
                        if (TinySQLGlobals.WHERE_DEBUG)
                            java.lang.SystemJ.outJ.println("Where phrase is " + wherePhrase);
                        if (foundKeyWord < upperField.length() - 4)
                            andOr = upperField.substring(foundKeyWord + 1, foundKeyWord + 3);
                        /*
                         *             Build a whereCondition Vector.  The elements are
                         *             as follows:
                         *             0 - left column object
                         *             1 - comparison  
                         *             2 - right column object
                         *             3 - status
                         *
                         *             The status values indicate which parts of the where
                         *             condition have been set.
                         */
                        whereCondition = new java.util.Vector<Object>();
                        for (j = 0; j < comparisons.Length; j++)
                        {
                            comp = comparisons[j];
                            foundComp = wherePhrase.toUpperCase().indexOf(comp);
                            if (foundComp > -1)
                            {
                                left = wherePhrase.substring(0, foundComp).trim();
                                leftColumn = new TsColumn(left, tableDefs, "WHERE");
                                whereCondition.addElement(leftColumn);
                                whereCondition.addElement(comp);
                                right = wherePhrase.substring(foundComp + comp.length()).trim();
                                if (comp.equals("IS"))
                                    right = "'" + right.toUpperCase() + "'";
                                rightColumn = new TsColumn(right, tableDefs, "WHERE");
                                whereCondition.addElement(rightColumn);
                                if (leftColumn.isConstant & rightColumn.isConstant)
                                    whereCondition.addElement("BOTH");
                                else if (leftColumn.isConstant)
                                    whereCondition.addElement("LEFT");
                                else if (rightColumn.isConstant)
                                    whereCondition.addElement("RIGHT");
                                else
                                    whereCondition.addElement("UNKNOWN");
                                break;
                            }
                        }
                        whereConditions.addElement(whereCondition);
                        /*
                         *             If this condition and the previous one are joined by an
                         *             AND keyword, add the condition to the existing Vector.
                         *             For an OR keyword, create a new entry in the whereClauseList.
                         */
                        if (andOr.equals("OR"))
                        {
                            whereClauseList.addElement(whereConditions);
                            whereConditions = new java.util.Vector<Object>();
                        }
                        startAt = foundKeyWord + andOr.length() + 2;
                    }
                }
            }
            /*
             *    Add the last where condition to the list.
             */
            if (whereConditions.size() > 0)
                whereClauseList.addElement(whereConditions);
            if (TinySQLGlobals.WHERE_DEBUG)
                java.lang.SystemJ.outJ.println("Where clause is \n" + toString());
        }
        /*
         * This method returns the column to build an index on.  This is very
         * primitive and only works on a single column that is compared to 
         * to a constant.
         */
        public java.util.Vector<Object> getIndexCondition(String inputTableName)
        {
            int i, j;
            java.util.Vector<Object> whereConditions;
            TsColumn leftColumn, rightColumn;
            Object whereObj;
            String objectType, comparison;
            java.util.Vector<Object> whereCondition;
            java.lang.StringBuffer outputBuffer = new java.lang.StringBuffer();
            for (i = 0; i < whereClauseList.size(); i++)
            {
                whereConditions = (java.util.Vector<Object>)whereClauseList.elementAt(i);
                for (j = 0; j < whereConditions.size(); j++)
                {
                    /*
                     *          Where conditions can be tinySQLWhere objects or String arrays.
                     */
                    whereObj = whereConditions.elementAt(j);
                    objectType = whereObj.getClass().getName();
                    if (objectType.endsWith("java.util.Vector"))
                    {
                        whereCondition = (java.util.Vector<Object>)whereObj;
                        leftColumn = (TsColumn)whereCondition.elementAt(0);
                        comparison = (String)whereCondition.elementAt(1);
                        rightColumn = (TsColumn)whereCondition.elementAt(2);
                        if (leftColumn.tableName.equals(inputTableName) &
                             rightColumn.isConstant & comparison.equals("="))
                        {
                            return whereCondition;
                        }
                        else if (leftColumn.tableName.equals(inputTableName) &
                           rightColumn.isConstant & comparison.equals("="))
                        {
                            return whereCondition;
                        }
                    }
                }
            }
            return (java.util.Vector<Object>)null;
        }
        /*
         * Clear all the non-constant values in all where conditions
         */
        public void clearValues(String inputTableName)
        {
            int i, j;
            java.util.Vector<Object> whereConditions;
            TsColumn leftColumn, rightColumn;
            Object whereObj;
            String objectType, status;
            java.util.Vector<Object> whereCondition;
            java.lang.StringBuffer outputBuffer = new java.lang.StringBuffer();
            for (i = 0; i < whereClauseList.size(); i++)
            {
                whereConditions = (java.util.Vector<Object>)whereClauseList.elementAt(i);
                for (j = 0; j < whereConditions.size(); j++)
                {
                    /*
                     *          Where conditions can be tinySQLWhere objects or String arrays.
                     */
                    whereObj = whereConditions.elementAt(j);
                    objectType = whereObj.getClass().getName();
                    if (objectType.endsWith("tinySQLWhere"))
                    {
                        ((TinySQLWhere)whereObj).clearValues(inputTableName);
                    }
                    else if (objectType.endsWith("java.util.Vector"))
                    {
                        whereCondition = (java.util.Vector<Object>)whereObj;
                        status = (String)whereCondition.elementAt(3);
                        if (status.equals("UNKNOWN")) continue;
                        /*
                         *             Check left side of condition
                         */
                        leftColumn = (TsColumn)whereCondition.elementAt(0);
                        if (leftColumn.clear(inputTableName))
                        {
                            if (status.equals("LEFT"))
                                whereCondition.setElementAt("UNKNOWN", 3);
                            else
                                whereCondition.setElementAt("RIGHT", 3);
                        }
                        /*
                         *             Check right side of condition
                         */
                        rightColumn = (TsColumn)whereCondition.elementAt(2);
                        if (rightColumn.clear(inputTableName))
                        {
                            if (status.equals("RIGHT"))
                                whereCondition.setElementAt("UNKNOWN", 3);
                            else
                                whereCondition.setElementAt("LEFT", 3);
                        }
                        if (TinySQLGlobals.WHERE_DEBUG)
                            java.lang.SystemJ.outJ.println("Where condition " + j
                            + " after clearing table " + inputTableName + " is\n"
                            + conditionToString(whereCondition));
                    }
                }
            }
        }
        public String toString()
        {
            int i, j;
            java.util.Vector<Object> whereConditions;
            Object whereObj;
            String objectType;
            java.util.Vector<Object> whereCondition;
            java.lang.StringBuffer outputBuffer = new java.lang.StringBuffer();
            for (i = 0; i < whereClauseList.size(); i++)
            {
                if (i > 0) outputBuffer.append("OR\n");
                whereConditions = (java.util.Vector<Object>)whereClauseList.elementAt(i);
                for (j = 0; j < whereConditions.size(); j++)
                {
                    if (j > 0) outputBuffer.append("AND\n");
                    /*
                     *          Where conditions can be tinySQLWhere objects or String arrays.
                     */
                    whereObj = whereConditions.elementAt(j);
                    objectType = whereObj.getClass().getName();
                    if (objectType.endsWith("tinySQLWhere"))
                    {
                        outputBuffer.append(((TinySQLWhere)whereObj).toString());
                    } if (objectType.endsWith("java.util.Vector"))
                    {
                        whereCondition = (java.util.Vector<Object>)whereObj;
                        outputBuffer.append(conditionToString(whereCondition) + "\n");
                    }
                }
            }
            return outputBuffer.toString();
        }
        /*
         * Format a where condition for display.
         */
        private String conditionToString(java.util.Vector<Object> inputWhereCondition)
        {
            String comparison, conditionStatus;
            TsColumn leftColumn, rightColumn;
            if (inputWhereCondition.size() < 4) return "";
            java.lang.StringBuffer outputBuffer = new java.lang.StringBuffer("WHERE ");
            leftColumn = (TsColumn)inputWhereCondition.elementAt(0);
            comparison = (String)inputWhereCondition.elementAt(1);
            rightColumn = (TsColumn)inputWhereCondition.elementAt(2);
            conditionStatus = (String)inputWhereCondition.elementAt(3);
            outputBuffer.append(leftColumn.getString() + " " + comparison
            + " " + rightColumn.getString() + " " + conditionStatus);
            return outputBuffer.toString();
        }
        /*
         * Given a column name, and a Hashtable containing tables, determine
         * which table "owns" a given column. 
         */
        private TinySQLTable getTableForColumn(java.util.Hashtable<Object, Object> tables, String inputColumn)
        {
            TinySQLTable tbl;
            java.util.Vector<Object> tableNames;
            java.util.Hashtable<Object, Object> columnInfo;
            String findColumn, tableAndAlias = (String)null, tableAlias;
            int i, dotAt;
            findColumn = inputColumn.toUpperCase();
            dotAt = findColumn.indexOf(".");
            tableNames = (java.util.Vector<Object>)tables.get("TABLE_SELECT_ORDER");
            if (dotAt > -1)
            {
                tableAlias = findColumn.substring(0, dotAt);
                try
                {
                    tableAndAlias = UtilString.findTableAlias(tableAlias, tableNames);
                }
                catch (Exception )
                {
                }
                if (tableAndAlias != (String)null)
                {
                    tbl = (TinySQLTable)tables.get(tableAndAlias);
                    if (tbl != (TinySQLTable)null) return tbl;
                }
            }
            else
            {
                for (i = 0; i < tableNames.size(); i++)
                {
                    tbl = (TinySQLTable)tables.get((String)tableNames.elementAt(i));
                    /*
                     *          Get the Hashtable containing column information, and see if it
                     *          contains the column we're looking for.
                     */
                    columnInfo = tbl.column_info;
                    if (columnInfo != (java.util.Hashtable<Object, Object>)null)
                        if (columnInfo.containsKey(findColumn)) return tbl;
                }
            }
            return (TinySQLTable)null;
        }
        /*
         * This method updates the where conditions that contain the input column and
         * returns the status of the entire where clause.
         */
        public String evaluate(String inputColumnName, String inputColumnValue)
        //throws tinySQLException
        {
            int i, j, result;
            TsColumn leftColumn, rightColumn;
            java.util.Vector<Object> whereConditions, whereCondition;
            Object whereObj;
            String objectType, comparison, conditionStatus;
            if (TinySQLGlobals.WHERE_DEBUG)
                java.lang.SystemJ.outJ.println("Evaluate where with " + inputColumnName
                + " = " + inputColumnValue);
            for (i = 0; i < whereClauseList.size(); i++)
            {
                whereConditions = (java.util.Vector<Object>)whereClauseList.elementAt(i);
                for (j = 0; j < whereConditions.size(); j++)
                {
                    /*
                     *          Where conditions can be tinySQLWhere objects or String arrays.
                     */
                    conditionStatus = "TRUE";
                    whereObj = whereConditions.elementAt(j);
                    objectType = whereObj.getClass().getName();
                    if (objectType.endsWith("tinySQLWhere"))
                    {
                        conditionStatus = ((TinySQLWhere)whereObj).evaluate(inputColumnName, inputColumnValue);
                    }
                    else if (objectType.endsWith("java.util.Vector"))
                    {
                        whereCondition = (java.util.Vector<Object>)whereObj;
                        /*
                         *             Check for updates on this column.  Update the status to 
                         *             reflect which parts of the where condition have been set.
                         */
                        leftColumn = (TsColumn)whereCondition.elementAt(0);
                        conditionStatus = (String)whereCondition.elementAt(3);
                        leftColumn.update(inputColumnName, inputColumnValue.trim());
                        leftColumn.updateFunctions();
                        if (leftColumn.isValueSet())
                        {
                            if (conditionStatus.equals("UNKNOWN"))
                                whereCondition.setElementAt("LEFT", 3);
                            else if (conditionStatus.equals("RIGHT"))
                                whereCondition.setElementAt("BOTH", 3);
                        }
                        rightColumn = (TsColumn)whereCondition.elementAt(2);
                        rightColumn.update(inputColumnName, inputColumnValue.trim());
                        rightColumn.updateFunctions();
                        if (rightColumn.isValueSet())
                        {
                            if (conditionStatus.equals("UNKNOWN"))
                                whereCondition.setElementAt("RIGHT", 3);
                            else if (conditionStatus.equals("LEFT"))
                                whereCondition.setElementAt("BOTH", 3);
                        }
                        if (TinySQLGlobals.WHERE_DEBUG)
                            java.lang.SystemJ.outJ.println(conditionToString(whereCondition));
                        /*
                         *             A where condition cannot be evaluated until both left and 
                         *             right values have been assigned.
                         */
                        conditionStatus = (String)whereCondition.elementAt(3);
                        if (conditionStatus.equals("UNKNOWN") |
                             conditionStatus.equals("LEFT") |
                             conditionStatus.equals("RIGHT")) continue;
                        /*
                         *             Evaluate this where condition.
                         */
                        conditionStatus = "TRUE";
                        /*
                         *             Any condition except IS NULL that involves a null
                         *             column must evaluate to FALSE.
                         */
                        comparison = (String)whereCondition.elementAt(1);
                        if (leftColumn.isNull() | rightColumn.isNull())
                        {
                            if (comparison.equals("IS"))
                            {
                                if (rightColumn.getString().equals("NULL"))
                                {
                                    whereCondition.setElementAt(conditionStatus, 3);
                                    continue;
                                }
                            }
                            whereCondition.setElementAt("FALSE", 3);
                            continue;
                        }
                        else
                        {
                            if (comparison.equals("IS"))
                            {
                                if (rightColumn.getString().equals("NOT NULL"))
                                {
                                    whereCondition.setElementAt("TRUE", 3);
                                    continue;
                                }
                                else if (rightColumn.getString().equals("NULL"))
                                {
                                    whereCondition.setElementAt("FALSE", 3);
                                    continue;
                                }
                                else
                                {
                                    throw new TinySQLException("Invalid WHERE condition "
                                    + rightColumn.getString());
                                }
                            }
                        }
                        /*
                         *             Evaluate all conditions other than NULL and NOT NULL
                         */
                        result = leftColumn.compareTo(rightColumn);
                        if (Utils.isCharColumn(leftColumn.type) |
                             Utils.isDateColumn(leftColumn.type))
                        {
                            /*
                             *                Character string comparisons.
                             */
                            if (comparison.equals("=") & result != 0)
                            {
                                conditionStatus = "FALSE";
                            }
                            else if (comparison.equals("<>") & result == 0)
                            {
                                conditionStatus = "FALSE";
                            }
                            else if (comparison.equals("!=") & result == 0)
                            {
                                conditionStatus = "FALSE";
                            }
                            else if (comparison.equals(">") & result <= 0)
                            {
                                conditionStatus = "FALSE";
                            }
                            else if (comparison.equals(">=") |
                                      comparison.equals("=>"))
                            {
                                if (result < 0) conditionStatus = "FALSE";
                            }
                            else if (comparison.equals("<") & result >= 0)
                            {
                                conditionStatus = "FALSE";
                            }
                            else if (comparison.equals("<=") |
                                      comparison.equals("=<"))
                            {
                                if (result > 0) conditionStatus = "FALSE";
                            }
                            else if (comparison.equalsIgnoreCase("LIKE"))
                            {
                                if (!leftColumn.like(rightColumn))
                                    conditionStatus = "FALSE";
                            }
                        }
                        else if (Utils.isNumberColumn(leftColumn.type))
                        {
                            /*
                             *                Numeric comparisons.
                             */
                            if (comparison.equals("=") & result != 0)
                                conditionStatus = "FALSE";
                            else if (comparison.equals("<>") & result == 0)
                                conditionStatus = "FALSE";
                            else if (comparison.equals(">") & result <= 0)
                                conditionStatus = "FALSE";
                            else if (comparison.equals("<") & result >= 0)
                                conditionStatus = "FALSE";
                            else if (comparison.equals("<=") & result > 0)
                                conditionStatus = "FALSE";
                            else if (comparison.equals("=<") & result > 0)
                                conditionStatus = "FALSE";
                            else if (comparison.equals(">=") & result < 0)
                                conditionStatus = "FALSE";
                            else if (comparison.equals("=>") & result < 0)
                                conditionStatus = "FALSE";
                        }
                        whereCondition.setElementAt(conditionStatus, 3);
                        if (TinySQLGlobals.WHERE_DEBUG)
                            java.lang.SystemJ.outJ.println("Where condition " + j + " evaluation:\n"
                            + conditionToString(whereCondition));
                    }
                }
            }
            return getStatus();
        }
        /*
         * This method evaluates the status of the entire where clause.
         */
        public String getStatus()
        {
            int i, j;
            java.util.Vector<Object> whereConditions;
            Object whereObj;
            String objectType, andStatus, orStatus, conditionStatus;
            java.util.Vector<Object> whereCondition;
            orStatus = "FALSE";
            for (i = 0; i < whereClauseList.size(); i++)
            {
                /*
                 *       The AND operator is applied to the whereConditions
                 */
                whereConditions = (java.util.Vector<Object>)whereClauseList.elementAt(i);
                andStatus = "TRUE";
                for (j = 0; j < whereConditions.size(); j++)
                {
                    /*
                     *          Where conditions can be tinySQLWhere objects or String arrays.
                     */
                    whereObj = whereConditions.elementAt(j);
                    objectType = whereObj.getClass().getName();
                    if (objectType.endsWith("tinySQLWhere"))
                    {
                        andStatus = ((TinySQLWhere)whereObj).getStatus();
                        if (andStatus.equals("FALSE")) break;
                    }
                    else if (objectType.endsWith("java.util.Vector"))
                    {
                        whereCondition = (java.util.Vector<Object>)whereObj;
                        /*
                         *             If any AND condition is FALSE, the entire where condition 
                         *             is FALSE.
                         */
                        conditionStatus = (String)whereCondition.elementAt(3);
                        if (conditionStatus.equals("FALSE"))
                        {
                            andStatus = "FALSE";
                            break;
                            /*
                             *             If any AND condition is UNKNOWN, LEFT, or RIGHT, the entire
                             *             where condition is UNKNOWN.
                             */
                        }
                        else if (conditionStatus.equals("UNKNOWN") |
                                  conditionStatus.equals("LEFT") |
                                  conditionStatus.equals("RIGHT"))
                        {
                            andStatus = "UNKNOWN";
                        }
                    }
                }
                /*
                 *       If any OR condition is true, the entire where condition
                 *       is true
                 */
                if (andStatus.equals("TRUE"))
                {
                    orStatus = "TRUE";
                    break;
                }
                else if (andStatus.equals("UNKNOWN"))
                {
                    orStatus = "UNKNOWN";
                }
            }
            if (TinySQLGlobals.WHERE_DEBUG)
                java.lang.SystemJ.outJ.println("Return status " + orStatus);
            return orStatus;
        }
    }
}