# 用来验证文件的crc、md5或sha256是否正确
       
## 参数介绍：
  1. 参数`-v`，显示版本信息和全部可用参数列表,之后无论是否指定其他参数直接退出。  
     ~~~
        filekey.exe -v
     ~~~
  2. 参数`-0`，只显示是否匹配后退出。  
     ~~~
        filekey.exe -0 file.dat bfa2dfbf5e3208ceebaf268e3bb8896e6dcbeb7af6d2c56d7f48c2fd849a3d1e
     ~~~
  3. 参数`-t`，只显示文件类型。  
     ~~~
        filekey.exe -t
     ~~~
  4. 参数`-cms`，分别`-c`只显示crc，`-m`只显示md5，`-s`只显示sha256。可以单独使用，也可以任意组合使用，不分先后顺序。同时指定三个与不指定任何一个效果相同。  
     ~~~
        filekey.exe -cms
        filekey.exe -ms
        filekey.exe -s
        filekey.exe -0s file.dat bfa2dfbf5e3208ceebaf268e3bb8896e6dcbeb7af6d2c56d7f48c2fd849a3d1e
     ~~~

## 使用方法：
  1. 不带参数运行，在交互界面根据提示输入需要计算key的文件或文件夹。如果是文件夹，那么此文件夹下的所有文件（不含子目录）都会被计算。然后根据提示输入需要比对的key值或key文件，程序将显示详情并提示是否匹配。
     ~~~
        filekey.exe
     ~~~
  2. 执行时跟一个文件或文件夹，如果包含空格需要在两端加双引号。可以使用`--File <FilePath>`或`--Directory <DirectoryPath>`明确指定文件或文件夹路径。程序将扫描指定并显示指定的文件信息，然后自动退出。  
     ~~~
        filekey.exe file.dat
        filekey.exe --File file.dat
        filekey.exe path
        filekey.exe --Directory path
        filekey.exe "C:\Program Files\file.dat"
     ~~~
  3. 执行时跟一个文件并在空格后跟一个key值，程序将比对指定文件与指定key值，显示是否匹配后退出。  
     ~~~
        filekey.exe file.dat bfa2dfbf5e3208ceebaf268e3bb8896e6dcbeb7af6d2c56d7f48c2fd849a3d1e
     ~~~
  4. 执行时跟一个文件并在空格后跟一个包含key值的文件，程序将计算第一个文件的key值并与指定key文件中记录的值匹配，显示是否匹配到文件中第几行第几列的key值后退出。  
     ~~~
        filekey.exe file.dat keys.txt
     ~~~
  5. 执行时使用`--Equals <FilePath>`指定一个文件，获得这个文件的sha256值，当作key值与指定文件或文件夹中的文件匹配。  
     ~~~
        filekey.exe file1.dat --Equals file2.dat
        filekey.exe --Directory path --Equals file.dat -0s
     ~~~
  
## 多语言：
  程序运行时根据系统语言显示中英文，不支持的语言系统默认显示英文。可以使用`--Language <zh|en|其他>`指定特定语言，指定其他时需要程序运行目录有`language_其他.txt`文件，未找到时显示英文模板及错误提示后退出。
  ~~~
     filekey.exe --Language zh
  ~~~

## 系统：
  使用.net8开发，应该支持多系统，windows，linux，macos。

  
