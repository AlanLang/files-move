# files-move
批量更新上传的压缩包里面的所有文件，根据一个配置文件来分配目录，如果目标目录里已有该文件则先把文件移动到备份文件夹里，避免丢失原文件。
## 使用方法
### 1. 新建配置文件 `config.json` 配置要移动的文件和目标路径
```
{
    "version": "1.0.0",
     "person": "开发者",
    "basepath":"E:\test",
    "files":[
        {"name":"text1.txt","path":""},
        {"name":"text2.txt","path":"demo"},
        {"name":"text3.txt","path":"demo\\demo1"}
    ]
}
```
### 2. 将配置文件和其他要移动的文件放在一起
![](http://oqdzx28cd.bkt.clouddn.com/18-1-10/77737370.jpg)

### 3. 将文件们压缩成 `zip` 
目前系统只支持zip类型的压缩文件

### 4. 运行本项目或者打开项目生成的 `exe` 文件
点击 `打开` 按钮，选择刚才配置的压缩文件即可。

### 5. 备注
本项目已引用 `Costura.Fody`，即生成的可执行文件为绿色单文件，可以随意拿到任何地方使用（仅限Windows）。