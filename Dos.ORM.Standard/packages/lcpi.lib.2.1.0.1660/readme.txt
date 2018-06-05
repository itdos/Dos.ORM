README - LCPI Instrumental Library.

=================================================================
Description:

This package contains common structures for LCPI software.

Home Page:
 http://www.ibprovider.com

Contact E-Mail:
 ibprovider@ibprovider.com

=================================================================
ChangeLog:

v2.1.0.1660
* tagDBDATE::ToDateTime_NoThrow
* tagDBTIMESTAMP::ToDateTime_NoThrow
* tagDBTIMESTAMPOFFSET::ToDateTimeOffset_NoThrow
  - Usage DB_E_DATAOVERFLOW instead DISP_E_OVERFLOW

v2.0.0.1654
* Redesign of public interface

v1.1.0.1348
* Reorganization
  - structure.t_str_formatter renamed to structure.StringFormatter
    + more exact implementation
  - structure.t_str_version renamed to structure.Version
  - structure.intptr_utils renamed to structure.IntPtrUtils
  - structure.uintptr_utils renamed to structure.UIntPtrUtils

v1.0.0.1282
* First public release
