using System;
using System.Reflection;
using System.Reflection.PortableExecutable;
using System.Reflection.Metadata;
using System.IO;

namespace AssemblyDemo.Examples
{
    /// <summary>
    /// PE (Portable Executable) 头和 CLR 头探索
    /// PE格式是Windows可执行文件的标准格式
    /// CLR头包含.NET特定的元数据信息
    /// </summary>
    public static class PEHeaderExplorer
    {
        /// <summary>
        /// 探索PE头信息
        /// PE头包含操作系统加载和执行程序所需的信息
        /// </summary>
        public static void ExplorePEHeaders()
        {
            Console.WriteLine("\n--- PE头和CLR头探索 ---");

            try
            {
                // 获取当前程序集的路径
                Assembly currentAssembly = Assembly.GetExecutingAssembly();
                string assemblyPath = currentAssembly.Location;

                Console.WriteLine($"正在分析程序集: {Path.GetFileName(assemblyPath)}");

                // 使用FileStream和PEReader读取PE文件
                using (FileStream stream = new FileStream(assemblyPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (PEReader peReader = new PEReader(stream))
                {
                    // 读取PE头信息
                    PEHeaders headers = peReader.PEHeaders;

                    // COFF头 (Common Object File Format)
                    Console.WriteLine("\n【COFF头信息】");
                    CoffHeader coffHeader = headers.CoffHeader;
                    Console.WriteLine($"  机器类型: {coffHeader.Machine}");
                    Console.WriteLine($"  节数量: {coffHeader.NumberOfSections}");
                    Console.WriteLine($"  时间戳: {DateTimeOffset.FromUnixTimeSeconds(coffHeader.TimeDateStamp).LocalDateTime}");
                    Console.WriteLine($"  特征标志: {coffHeader.Characteristics}");

                    // PE头
                    if (headers.PEHeader != null)
                    {
                        Console.WriteLine("\n【PE头信息】");
                        PEHeader peHeader = headers.PEHeader;
                        Console.WriteLine($"  魔数 (32/64位): {peHeader.Magic}");
                        Console.WriteLine($"  链接器版本: {peHeader.MajorLinkerVersion}.{peHeader.MinorLinkerVersion}");
                        Console.WriteLine($"  代码大小: {peHeader.SizeOfCode:N0} 字节");
                        Console.WriteLine($"  初始化数据大小: {peHeader.SizeOfInitializedData:N0} 字节");
                        Console.WriteLine($"  入口点地址: 0x{peHeader.AddressOfEntryPoint:X8}");
                        Console.WriteLine($"  基地址: 0x{peHeader.ImageBase:X}");
                        Console.WriteLine($"  节对齐: {peHeader.SectionAlignment}");
                        Console.WriteLine($"  文件对齐: {peHeader.FileAlignment}");
                        Console.WriteLine($"  子系统: {peHeader.Subsystem}");
                    }

                    // CLR头 (.NET特定)
                    if (headers.CorHeader != null)
                    {
                        Console.WriteLine("\n【CLR头信息】");
                        CorHeader corHeader = headers.CorHeader;
                        Console.WriteLine($"  CLR运行时版本: {corHeader.MajorRuntimeVersion}.{corHeader.MinorRuntimeVersion}");
                        Console.WriteLine($"  元数据目录地址: 0x{corHeader.MetadataDirectory.RelativeVirtualAddress:X8}");
                        Console.WriteLine($"  元数据目录大小: {corHeader.MetadataDirectory.Size:N0} 字节");
                        Console.WriteLine($"  标志: {corHeader.Flags}");
                        Console.WriteLine($"  入口点标记: 0x{corHeader.EntryPointTokenOrRelativeVirtualAddress:X8}");
                        
                        // 检查标志
                        Console.WriteLine("\n  CLR标志分析:");
                        Console.WriteLine($"    ILOnly (仅IL代码): {(corHeader.Flags & CorFlags.ILOnly) != 0}");
                        Console.WriteLine($"    32BitRequired (需要32位): {(corHeader.Flags & CorFlags.Requires32Bit) != 0}");
                        Console.WriteLine($"    32BitPreferred (首选32位): {(corHeader.Flags & CorFlags.Prefers32Bit) != 0}");
                        Console.WriteLine($"    StrongNameSigned (强名称签名): {(corHeader.Flags & CorFlags.StrongNameSigned) != 0}");
                    }

                    // 节头信息
                    Console.WriteLine("\n【节头信息】");
                    var sectionHeaders = headers.SectionHeaders;
                    Console.WriteLine($"  节数量: {sectionHeaders.Length}");
                    foreach (var section in sectionHeaders)
                    {
                        Console.WriteLine($"\n  节名称: {section.Name}");
                        Console.WriteLine($"    虚拟地址: 0x{section.VirtualAddress:X8}");
                        Console.WriteLine($"    虚拟大小: {section.VirtualSize:N0} 字节");
                        Console.WriteLine($"    原始数据大小: {section.SizeOfRawData:N0} 字节");
                        Console.WriteLine($"    特征: {section.SectionCharacteristics}");
                    }

                    // 元数据读取器
                    if (peReader.HasMetadata)
                    {
                        Console.WriteLine("\n【元数据信息】");
                        MetadataReader metadataReader = peReader.GetMetadataReader();
                        
                        Console.WriteLine($"  TypeDefinition数量: {metadataReader.TypeDefinitions.Count}");
                        Console.WriteLine($"  TypeReference数量: {metadataReader.TypeReferences.Count}");
                        Console.WriteLine($"  MethodDefinition数量: {metadataReader.MethodDefinitions.Count}");
                        Console.WriteLine($"  FieldDefinition数量: {metadataReader.FieldDefinitions.Count}");
                        Console.WriteLine($"  PropertyDefinition数量: {metadataReader.PropertyDefinitions.Count}");
                        Console.WriteLine($"  EventDefinition数量: {metadataReader.EventDefinitions.Count}");

                        // 读取模块信息
                        ModuleDefinition module = metadataReader.GetModuleDefinition();
                        Console.WriteLine($"\n  模块名称: {metadataReader.GetString(module.Name)}");
                        Console.WriteLine($"  MVID: {metadataReader.GetGuid(module.Mvid)}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"读取PE头时发生错误: {ex.Message}");
            }
        }
    }
}
