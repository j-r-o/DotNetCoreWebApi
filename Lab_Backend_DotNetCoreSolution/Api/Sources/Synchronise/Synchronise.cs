using System;
using System.Text;
using System.IO;

namespace Api.Synchronise {
  public class Synchronise {
    public Synchronise() {
      FileSystemWatcher watcher = new FileSystemWatcher(@"/");
      watcher.IncludeSubdirectories = true;
      watcher.Filter = "";
      watcher.Renamed += new RenamedEventHandler(renamed);
      watcher.Deleted += new FileSystemEventHandler(changed);
      watcher.Changed += new FileSystemEventHandler(changed);
      watcher.Created += new FileSystemEventHandler(changed);
      watcher.EnableRaisingEvents = true;

      Console.ReadKey();
   }

    private static void renamed(object sender, RenamedEventArgs e) {
      Console.WriteLine(DateTime.Now + ": " + 
          e.ChangeType + " " + e.FullPath);
    }

    private static void changed(object sender, FileSystemEventArgs e) {
      Console.WriteLine(DateTime.Now + ": " + 
          e.ChangeType + " " + e.FullPath);
    }
  }
}