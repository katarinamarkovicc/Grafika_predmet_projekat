   M S B A M L   `   `   `  ����>  :PZ2, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullclr-namespace:PZ2PZ2  5   	       PZ2.MainWindow   5   	   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   X TXceed.Wpf.Toolkit, Version=4.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f44xctk)http://schemas.xceed.com/wpf/xaml/toolkit  5   	   localclr-namespace:PZ2  5   	     � Title$  
MainWindow��5	   	   $	��960��6   $
��1280��6)    J� CommandBindings 5
      )  #System.Windows.Input.CommandBinding  5   
   -   5           Command$( "local:RoutedCommands.SelectEllipse��5      6�     5   
   -   5        $( "local:RoutedCommands.SelectPolygon��5      6�     5   
   -   5        $% local:RoutedCommands.SelectText��5      6�     5   
   -   5        $ local:RoutedCommands.Undo��5      6u     5   
   -   5        $ local:RoutedCommands.Redo��5      6u   .��5      ]� .+�5   
   �� $��0,5,0,0q�6    �� Orientation$ 
Horizontal=�6&   $	��Top=�6?   
�������.+�5      �� -   5         ��Name$ 	ElipseBtn��5      $��Ellipse��6(   $��86��6:   $��3q�6F   $(��"local:RoutedCommands.SelectEllipse��6Q   
��� ���6~   �� 5      -   5        $ 
PolygonBtn��5      $��Polygon��6)   $��86��6;   $��3q�6F   $(��"local:RoutedCommands.SelectPolygon��6Q   
��� ���6~   �� 5      -   5        $ TextBtn��5      $
��Text��6&   $��86��65   $��3q�6@   $%��local:RoutedCommands.SelectText��6K   
��� ���6u   �� 5      -	   5        $ UndoBtn��5      $
��Undo��6&   $��86��65   $��3q�6@   $��local:RoutedCommands.Undo��6K   
��� ���6q   �� 5      -
   5        $ RedoBtn��5      $
��Redo��6&   $��86��65   $��3q�6@   $��local:RoutedCommands.Redo��6K   
��� ���6o   �� 5      -   5        $ ClearBtn��5      $��Clear��6'   $��86��67   $��3q�6C   
��� ���6e   �� 5      -   5        $ Unselect��5      $��Unselect��6'   $��86��6:   $��3q�6F   
��� ���6h   �� 5      -   5        $ 
ScreenShot��5      $��Take a picture��6)   $��86��6B   $��3q�6N   
��� ���6r   �� 5      -   5         ��Name$ checkInactive��5      $��Inactive network��6.   $��Center=�6I   $��
5,8,30,7.4q�6d    d� 	IsChecked$
 TrueY�6x   6�   �� 5      -   5        $ checkLineColor��5      $+��%Change line color based on resistance��6/   $��Center=�6_   $
��10,0q�6z   $ FalseY�6�   {�. 6�   o� 5      -   5         o�Name$
 Menu��5      $	��100��6"   $��35��6.    �� Left$	 563��6:   
	 �� Top$		 376��6L   
�������.@�5       m� $��Change Colour��6   .@�5!      #  Xceed.Wpf.Toolkit.ColorPicker  -   5        $��ClrPcker_Background��5!   '   6�   5"      m� 5#      $��Image��6   .@�5$      �� -   5        $ buttonZaSliku��5$      .��6I   Choose a image6Y   5%      5&      5'      �� 5(   
   $��Bottom=�6   $	��100��6)   
�������.+�5)      �� -   5        
 ��Name$
 
Substation��5)      $��Substation - ��6(   $ 10��6@   $��23��6Q   $��78��6]   $	 2��6h   �� 5*      -   5        $
 Switch��5*      $��	Switch - ��6$   $ 10��68   $��23��6I   $��68��6U   $	 21��6`   �� 5+      -   5        $

 Node��5+      $��Node - ��6"   $ 10��64   $��23��6E   $��68��6Q   $	 41��6\   �� 5,      -   5        $

 Line��5,      $��Line - ��6"   $ 10��64   $��23��6E   $��68��6Q   $	 64��6\   A� 5.      
��� � �$��11��6#   $ 81��6/   
���   �$	 11��6O   $��12��6_    J� RenderTransformOrigin$ 0.5,0.5&�6j   A� 50      
���  ��$��11��6!   $ 59��6-   
���   �$	 30��6M   $��12��6]   $ 0.194,0.606&�6h   A� 53      
����  �$��11��6"   $ 54��6.   
���   �$	 50��6N   $��12��6^   $ 0.5,0.5&�6i   �� 56      
���   �$��2��6%   $ 47��60   
���   �$��21��6P   $	 78��6[   �� 57      -   5        $ OpenFileBtn��57      $��Load Xml��6*   $	 117��6=   $	 60��6O   $��75��6_   6j   58      �� 59   
   -   5         ��Name$ slider��59       � Minimum$ .1Q�6!    � Maximum$ 10Q�6.    � Value$ 1Q�6;   $	��Top=�6E   �� 5:   
   -   5        $
��Auto=�5:      $
��Auto=�6<   |�.  .��5;      �� 
���   �$��1q�6)   $��Center=�6=   $��Center=�6Z   ._�5<      �� -   5        $��
GridCanvas��5<   5   $
��5000��6   $
��5000��6&   
������� � LayoutTransform 5=      �� 5>       �� ScaleX 6)   ��  �� ElementName$ slider�� �� Path$ Value� �� ScaleY 5?   !   �� $ slider��$ Value�5B      5C      5D      5E      5F      