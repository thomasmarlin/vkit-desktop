   M S B A M L   `   `   `  ����   VkitDesktop    VkitDesktop.MainWindow   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml       � Title$"  SWCCG Virtual Slip Generator��$	��506��$	��749��
�������.��� -   .+���  �� Orientation$ Vertical=�$��5,0,0,0q�.+��� $��Instructions:��$
��Bold"��� $y��s1.  All available Virtual Cards are available on the left. Enter text in the filter textbox to narrow down results.���� $}��w2.  Double-Click the card or press 'Add BB' or 'Add WB' to add cards to your list. BB=Black Border, WB = White Border. ���� $W��Q3.  Select a card from the right and select 'Remove' to remove it from your list.���� $`��Z4.  Click 'Generate PDF...' to generate a PDF containing all cards in the right-hand list.���� $}��wNote:  The 'Import from txt...' button will import vcards from a Holotable-exported deck list. (not perfect, but close)��� $��	0,100,0,0q� � ColumnDefinitions �� $��* ��� $	��100 ��� $��* �.+��� $��0,2,0,0q�$��3��$��Stretch=�$	��Top=�$��1��
�������� $��0��.+��� -   $��myFilterText��$��23��$��Stretch=�$��5,27,5,0q�$	��Top=��� -   $��label1��$��Filter by Name:��$��28��$
��Left=�$��3,6,0,0q�$	��Top=�$	��134���� -   $��myFilteredVcards��$��Stretch=�$��	5,56,5,45q�$��Stretch=� �� DisplayMemberPath$ CardName�� �� SelectionMode$ Extended=�� $��1��.+��� -   $��myMoveRightButton��$��Add BB��$
��Bold"�$��23��$��Stretch=�$��	2,-50,2,0q�$��Center=��� -   $"��myMoveWhiteBorderRightButton��$��	Add White��$
��Bold"�$��23��$��Stretch=�$��2,0,2,0q�$��Center=��� -   $��myMoveLeftButton��$��Remove��$
��Bold"�$��23��$��Stretch=�$��2,50,2,0q�$��Center=��� -   $��myOptionsButton��$��Options��$��23��$��Stretch=�$��	0,338,0,0q�$	��Top=��� -	   $��myCubeFeaturesCheckbox��$��CUBE-Features��$��16��$
��Left=�$��	0,306,0,0q�$	��Top=�$��95�� d� 	IsChecked$
 TrueY�� $��2��.+��� -
   $��mySelectedVcards��$��Stretch=�$��	5,56,5,45q�$��Stretch=�$ CardNameWithBorder��$ Extended=� � ContextMenu �� .@�m� -   $��Delete��m� -   $ ��Switch To Alternate Border��m� -   $��Add / Edit Side Text���� -   $��myWriteButton��$��Generate PDF...��$��23��$��Right=�$��0,0,5,5q�$��Bottom=�$��95���� -   $��label2��$��Cards to print:��$��28��$
��Left=�$��1,32,0,0q�$	��Top=��� -   $��myImportButton��$��Import Holotable Deck...��$��23��$
��Left=�$��
172,27,0,0q�$	��Top=�$	��141���� -   $��myExportListButton��$��Export List...��$��23��$
��Left=�$��
11,338,0,0q�$	��Top=�$��75���� -   $��myImportListButton��$��Import List...��$��23��$
��Left=�$��
92,338,0,0q�$	��Top=�$��75���� $��3��
�������$��0,0,0,35q�$��1��$��Stretch=�$��Bottom=��� $5��/Comments/Questions:  hyvee_doughboy@hotmail.com��$��5,0,0,5q�$
��Left=�$��Bottom=�