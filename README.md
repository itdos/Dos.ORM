# Dos.ORM
Dos.ORM（原Hxj.Data）于2009年发布，并发布实体生成工具。在开发过程参考了多个ORM框架，特别是NBear，MySoft、EF、Dapper等。吸取了他们的一些精华，加入自己的新思想。该组件已在上百个成熟企业项目中应用。<br>
<b>为什么选择Dos.ORM（原Hxj.Data）?</b><br>
*）上手简单、使用方便、功能强大、高性能（与Dapper媲美，接近手写Sql）。<br>
*）体积小（不到150kb，仅一个dll）<br>
*）完美支持Sql Server(2000至最新版),MySql,Oracle,Access,Sqlite数据库。<br>
*）支持Lambda表达式写法。<br>
*）不需要像NHibernate的XML配置，不需要像EF的各种数据库驱动。<br>
*）众多成熟企业项目已应用此框架。<br>
*）遵循MIT开源协议，除不允许改名，其它随意定制修改。<br>

作者博客：http://www.cnblogs.com/huxj/<br>
官方网站：http://ITdos.com/Dos/ORM/Index.html<br>
交流QQ群：60831381<br>

<b>一句代码配置：</b><br>
public class DB{public static readonly DbSession Context = new DbSession("connectionStringsName");}<br>
<b>开始使用：</b><br>
DB.Context.From<Dos.Model.TableName>()<br>
    .Select(d => new { d.id, d.price })    //SELECT id,price FROM TableName<br>
    .Where(d => (d.id == 2 && d.name != "itdos" <br>
                    && d.name.In("com","net","cn")) || d.sex != null)    <br>
          // WHERE (id = 2 AND name <> 'itdos' AND name IN('com','net','cn')) OR sex IS NOT NULL<br>
    .GroupBy(d => new { d.name, d.sex })    //GROUP BY name,sex<br>
    .OrderBy(d => new { d.createTime, d.name })    //ORDER BY createTime,name<br>
    .Having(d => d.name != '')    //HAVING name<>''<br>
    .Top(5)<br>
    .Page(10, 2)<br>
    .ToList();<br>
