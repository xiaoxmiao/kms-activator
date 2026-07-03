using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Management;
using Microsoft.Win32;

class KmsActivator
{
    static string Run(string cmd, string args)
    {
        try
        {
            var psi = new ProcessStartInfo(cmd, args)
            {
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };
            var p = Process.Start(psi);
            p.WaitForExit();
            return p.StandardOutput.ReadToEnd() + p.StandardError.ReadToEnd();
        }
        catch { return ""; }
    }

    static string SafeString(object o)
    {
        if (o == null || o is DBNull) return "";
        return o.ToString().Trim();
    }

    static int SafeInt(object o)
    {
        if (o == null || o is DBNull) return 0;
        try { return Convert.ToInt32(o); }
        catch { return 0; }
    }

    struct Entry { public string Pattern; public string Key; public Entry(string p, string k) { Pattern = p; Key = k; } }

    static Entry[] _table = new Entry[]
    {
        new Entry("企业版 G N||Enterprise G N", "44RPN-FTY23-9VTTB-MP9BX-T84FV"),
        new Entry("企业版 G||Enterprise G", "YYVX9-NTFWV-6MDM3-9PT4T-4M68B"),
        new Entry("企业版 N||Enterprise N", "DPH2V-TTNVB-4X9Q3-TJR4H-KHJW4"),
        new Entry("企业版||Enterprise", "NPPR9-FWDCX-D2C8J-H872K-2YT43"),
        new Entry("专业工作站 N||Pro Workstation N||Workstation N", "9FNHH-K3HBT-3W4TD-6383H-6XYWF"),
        new Entry("专业工作站||Pro Workstation||Workstation", "NRG8B-VKK3Q-CXVCJ-9G2XF-6Q84J"),
        new Entry("专业教育版 N||Pro Education N", "YVWGF-BXNMC-HTQYQ-CPQ99-66QFC"),
        new Entry("专业教育版||Pro Education", "6TP4R-GNPTD-KYYHQ-7B7DP-J447Y"),
        new Entry("专业版 N||Pro N", "MH37W-N47XK-V7XM9-C7227-GCQG9"),
        new Entry("专业版||Pro", "W269N-WFGWX-YVC9B-4J6C9-T83GX"),
        new Entry("教育版 N||Education N", "2WH4N-8QGBV-H22JP-CT43Q-MDWWJ"),
        new Entry("教育版||Education", "NW6C2-QMPVW-D7KKK-3GKT6-VCFB2"),
        new Entry("家庭版 N||Home N", "3KHY7-WNT83-DGQKR-F7HPR-844BM"),
        new Entry("单语言||Single Language", "7HNRX-D7KGG-3K4RQ-4WPJ4-YTDFH"),
        new Entry("特定国家||Country Specific", "PVMJN-6DFY6-9CCP6-7BKTT-D3WVR"),
        new Entry("家庭版||Home", "TX9XD-98N7V-6WMQ6-BX7FG-H8Q99"),
        new Entry("LTSC 2024 N||LTSC 2021 N||LTSC 2019 N||LTSC N", "92NFX-8DJQP-P6BBQ-THF9C-7CG2H"),
        new Entry("LTSC 2024||LTSC 2021||LTSC 2019||2019 LTSC||LTSC", "M7XTQ-FN8P6-TTKYV-9D4CC-J462D"),
        new Entry("LTSB 2016 N||LTSB 2016", "QFFDN-GRT3P-VKWWX-X7T3R-8B639"),
        new Entry("LTSB 2016", "DCPHK-NFMTC-H88MJ-PFHPY-QJ4BJ"),
        new Entry("LTSB 2015 N||LTSB 2015", "2F77B-TNFGY-69QQF-B8YKP-D69TJ"),
        new Entry("LTSB 2015", "WNMTR-4C88C-JK8YV-HQ7T2-76DF9"),
        new Entry("ARM", "7NX88-X6YM3-9Q3YT-CCGBF-KBVQF"),
        new Entry("CoreConnected N||CoreConnectedN", "JQNT7-W63G4-WX4QX-RD9M9-6CPKM"),
        new Entry("CoreConnected Country||CoreConnectedCountry", "QQMNF-GPVQ6-BFXGG-GWRCX-7XKT7"),
        new Entry("CoreConnected Single||CoreConnectedSingle", "FTNXM-J4RGP-MYQCV-RVM8R-TVH24"),
        new Entry("CoreConnected", "DJMYQ-WN6HG-YJ2YX-82JDB-CWFCW"),
        new Entry("学生专业版 N||学生专业", "8G9XJ-GN6PJ-GW787-MVV7G-GMR99"),
        new Entry("学生专业版||学生专业", "YNXW3-HV3VB-Y83VG-KPBXM-6VH3Q"),
        new Entry("嵌入式 A||Embedded A", "GN2X2-KXTK6-P92FR-VBB9G-PDJFP"),
        new Entry("嵌入式 E||Embedded E", "XCNC9-BPK3C-KCCMD-FTDTC-KWY4G"),
        new Entry("嵌入式||Embedded", "XY4TQ-CXNVJ-YCT73-HH6R7-R897X"),
        new Entry("专业单语言||Pro Single Language", "NFDD9-FX3VM-DYCKP-B8HT8-D9M2C"),
        new Entry("专业单语言 N||Pro Single Language N", "8Q36Y-N2F39-HRMHT-4XW33-TCQR4"),
        new Entry("专业版WMC||Pro WMC", "NKPM6-TCVPT-3HRFX-Q4H9B-QJ34Y"),
        new Entry("Server 2025||服务器 2025", "D764K-2NDRG-47G6V-P2R7X-P8H6J"),
        new Entry("Server 2022||服务器 2022", "WX4NM-KYWYW-QJJR4-XV3QB-6VM33"),
        new Entry("2019 Datacenter||2019 数据中心", "WMDGN-G9PQG-XVVXX-R3X43-63DFG"),
        new Entry("2019 Standard||2019 标准版", "N69G4-B89J2-4G8F4-WWYCC-J464C"),
        new Entry("2019 Essentials||2019  essentials", "WVDHN-86M7X-466P6-VHXV7-YY726"),
        new Entry("2016 Datacenter||2016 数据中心", "CB7KF-BWN84-R7R2Y-793K2-8XDDG"),
        new Entry("2016 Standard||2016 标准版", "WC2BQ-8NRM3-FDDYY-2BFGV-KHKQY"),
        new Entry("2016 Essentials||2016  essentials", "JCKRF-N37P4-C2D82-9YXRT-4M63B"),
        new Entry("2012 R2 Datacenter||2012 R2 数据中心", "W3GGN-FT8W3-Y4M27-J84CP-Q3VJ9"),
        new Entry("2012 R2 Standard||2012 R2 标准版", "D2N9P-3P6X9-2R39C-7RTCD-MDVJX"),
        new Entry("2012 R2 Essentials||2012 R2 essentials", "KNC87-3J2TX-XB4WP-VCPJV-M4FWM"),
        new Entry("Server 2012||服务器 2012", "BN3D2-R7TKB-3YPBD-8DRP2-27GG4"),
        new Entry("2012 N||2012 N", "8N2M2-HWPGY-7PGT9-HGDD8-GVGGY"),
        new Entry("2012 单语言||2012 Single Language", "2WN2H-YGCQR-KFX6K-CD6TF-84YXQ"),
        new Entry("2012 特定国家||2012 Country Specific", "4K36P-JN4VD-GDC6V-KDT89-DYFKP"),
        new Entry("2008 R2 Datacenter||2008 R2 数据中心", "74YFP-3QFB3-KQT8W-PMXWJ-7M648"),
        new Entry("2008 R2 Enterprise||2008 R2 企业版", "489J6-VHDMP-X63PK-3K798-CPX3Y"),
        new Entry("2008 R2 Standard||2008 R2 标准版", "YC6KT-GKW9T-YTKYR-T4X34-R7VHC"),
        new Entry("2008 R2 HPC", "TT8MH-CG224-D3D7Q-498W2-9QCTX"),
        new Entry("2008 R2 Web", "6TPJF-RBVHG-WBW2R-86QPH-6RTM4"),
        new Entry("2008 Datacenter||2008 数据中心", "7M67G-PC374-GR742-YH8V4-TCBY3"),
        new Entry("2008 Enterprise||2008 企业版", "YQGMW-MPWTJ-34KDK-48M3W-X4Q6V"),
        new Entry("2008 Standard||2008 标准版", "TM24T-X9RMF-VWXK6-X8JC9-BFGM2"),
        new Entry("2008 HPC", "RCTX3-KWVHP-BR6TB-RB6DM-6X7HP"),
        new Entry("2008 Web", "WYR28-R7TFJ-3X2YQ-YCY4H-M249D"),
        new Entry("8.1 企业版 N||8.1 Enterprise N", "TT4HM-HN7YT-62K67-R4QCF-PHGKY"),
        new Entry("8.1 企业版||8.1 Enterprise", "MHF9N-XY6XB-WVXMC-BTDCT-MKKG7"),
        new Entry("8.1 专业版 N||8.1 Pro N", "HMCNV-VVBFX-7HMBH-CTY9B-B4FXY"),
        new Entry("8.1 专业版||8.1 Pro", "GCRJD-8NW9H-F2CDX-CCM8D-9D6T9"),
        new Entry("8.1 专业版WMC||8.1 Pro WMC", "789NJ-TQK6T-6XTH8-J39CJ-J8D3P"),
        new Entry("8.1 嵌入式||8.1 Embedded", "NMMPB-38DD4-R2823-62W8D-VXKJB"),
        new Entry("8.1 嵌入式 A||8.1 Embedded A", "VHXM3-NR6FT-RY6RT-CK882-KW2CJ"),
        new Entry("8.1 嵌入式 E||8.1 Embedded E", "FNFKF-PWTVT-9RC8H-32HB2-JB34X"),
        new Entry("Windows 8 企业版 N||Windows 8 Enterprise N", "JMNMF-RHW7P-DMY6X-RF3DR-X2BQT"),
        new Entry("Windows 8 企业版||Windows 8 Enterprise", "32JNW-9KQ84-P47T8-D8GGY-CWCK7"),
        new Entry("Windows 8 专业版 N||Windows 8 Pro N", "XCVCF-2NXM9-723PB-MHCB7-2RYQQ"),
        new Entry("Windows 8 专业版||Windows 8 Pro", "NG4HW-VH26C-733KW-K6F98-J8CK4"),
        new Entry("Windows 8 专业版WMC||Windows 8 Pro WMC", "GNBB8-YVD74-QJHX6-27H4K-8QHDG"),
        new Entry("Windows 7 企业版 E||Windows 7 Enterprise E", "C29WB-22CC8-VJ326-GHFJW-H9DH4"),
        new Entry("Windows 7 企业版 N||Windows 7 Enterprise N", "YDRBP-3D83W-TY26F-D46B2-XCKRJ"),
        new Entry("Windows 7 企业版||Windows 7 Enterprise", "33PXH-7Y6KF-2VJC9-XBBR8-HVTHH"),
        new Entry("Windows 7 专业版 E||Windows 7 Professional E", "W82YF-2Q76Y-63HXB-FGJG9-GF7QX"),
        new Entry("Windows 7 专业版 N||Windows 7 Professional N", "MRPKT-YTG23-K7D7T-X2JMM-QY7MG"),
        new Entry("Windows 7 专业版||Windows 7 Professional", "FJ82H-XT6CR-J8D7P-XQJJ2-GPDD4"),
        new Entry("Windows 7 旗舰版||Windows 7 Ultimate", "49PB6-6BJ6Y-KHGCQ-7DDY6-TF7CD"),
        new Entry("Windows 7 精简版||Windows 7 Starter", "273P4-GQ8V6-97YYM-9YTHF-DC2VP"),
        new Entry("Windows 7 高级家庭版||Windows 7 Home Premium", "CQBVJ-9J697-PWB9R-4K7W4-2BT4J"),
        new Entry("Windows 7 普通家庭版||Windows 7 Home Basic", "2P6PB-G7YVY-W46VJ-BXJ36-PGGTG"),
        new Entry("Vista 企业版 N||Vista Enterprise N", "VTC42-BM838-43QHV-84HX6-XJXKV"),
        new Entry("Vista 企业版||Vista Enterprise", "VKK3X-68KWM-X2YGT-QR4M6-4BWMV"),
        new Entry("Vista 商业版 N||Vista Business N", "HMBQG-8H2RH-C77VX-27R82-VMQBT"),
        new Entry("Vista 商业版||Vista Business", "YFKBB-PQJJV-G996G-VWGXY-2V3X8"),
    };

    static string GetKey(string caption)
    {
        bool captionHasLTSC = caption.IndexOf("LTSC", StringComparison.OrdinalIgnoreCase) >= 0 ||
                               caption.IndexOf("LTSB", StringComparison.OrdinalIgnoreCase) >= 0;
StartSearch:
        for (int i = 0; i < _table.Length; i++)
        {
            string[] ors = _table[i].Pattern.Split(new string[] { "||" }, StringSplitOptions.None);
            foreach (string pattern in ors)
            {
                if (caption.Contains(pattern))
                {
                    string key = _table[i].Key;
                    // If caption contains LTSC/LTSB, skip entries that don't mention LTSC/LTSB in pattern
                    if (captionHasLTSC &&
                        pattern.IndexOf("LTSC", StringComparison.OrdinalIgnoreCase) < 0 &&
                        pattern.IndexOf("LTSB", StringComparison.OrdinalIgnoreCase) < 0)
                        continue;
                    bool isServerKey = key == "D764K-2NDRG-47G6V-P2R7X-P8H6J" ||
                                       key == "WX4NM-KYWYW-QJJR4-XV3QB-6VM33" ||
                                       key == "N69G4-B89J2-4G8F4-WWYCC-J464C" ||
                                       key == "WMDGN-G9PQG-XVVXX-R3X43-63DFG" ||
                                       key == "WVDHN-86M7X-466P6-VHXV7-YY726" ||
                                       key == "CB7KF-BWN84-R7R2Y-793K2-8XDDG" ||
                                       key == "WC2BQ-8NRM3-FDDYY-2BFGV-KHKQY" ||
                                       key == "JCKRF-N37P4-C2D82-9YXRT-4M63B" ||
                                       key == "D2N9P-3P6X9-2R39C-7RTCD-MDVJX" ||
                                       key == "W3GGN-FT8W3-Y4M27-J84CP-Q3VJ9" ||
                                       key == "KNC87-3J2TX-XB4WP-VCPJV-M4FWM" ||
                                       key == "BN3D2-R7TKB-3YPBD-8DRP2-27GG4" ||
                                       key == "8N2M2-HWPGY-7PGT9-HGDD8-GVGGY" ||
                                       key == "2WN2H-YGCQR-KFX6K-CD6TF-84YXQ" ||
                                       key == "4K36P-JN4VD-GDC6V-KDT89-DYFKP" ||
                                       key == "74YFP-3QFB3-KQT8W-PMXWJ-7M648" ||
                                       key == "489J6-VHDMP-X63PK-3K798-CPX3Y" ||
                                       key == "YC6KT-GKW9T-YTKYR-T4X34-R7VHC" ||
                                       key == "TT8MH-CG224-D3D7Q-498W2-9QCTX" ||
                                       key == "6TPJF-RBVHG-WBW2R-86QPH-6RTM4" ||
                                       key == "7M67G-PC374-GR742-YH8V4-TCBY3" ||
                                       key == "YQGMW-MPWTJ-34KDK-48M3W-X4Q6V" ||
                                       key == "TM24T-X9RMF-VWXK6-X8JC9-BFGM2" ||
                                       key == "RCTX3-KWVHP-BR6TB-RB6DM-6X7HP" ||
                                       key == "WYR28-R7TFJ-3X2YQ-YCY4H-M249D" ||
                                       key == "39BXF-X8Q23-P2WWT-38T2F-G3FPG" ||
                                       key == "22XQ2-VRXRG-P8D42-K34TD-G3QQC" ||
                                       key == "4DWFP-JF3DJ-B7DTH-78FJB-PDRHK" ||
                                       key == "DBGBW-NPF86-BJVTX-K3WKJ-MTB6V" ||
                                       key == "Y4TGP-NPTV9-HTC2H-7MGQ3-DV4TW" ||
                                       key == "K2XGM-NMBT3-2R6Q8-WF2FK-P36R2";
                    if (!isServerKey && caption.Contains("Server")) continue;
                    return key;
                }
            }
        }
        // If caption has LTSC/LTSB but no match was found, try again without the restriction
        if (captionHasLTSC)
        {
            captionHasLTSC = false;
            goto StartSearch;
        }
        return null;
    }

    // ============== Office detection & activation ==============

    struct OfficeKeyInfo
    {
        public string VersionLabel;
        public string OsppDir;
        public string Key;
        public string ProductLabel;
        public OfficeKeyInfo(string vl, string dir, string k, string pl)
        {
            VersionLabel = vl; OsppDir = dir; Key = k; ProductLabel = pl;
        }
    }

    static OfficeKeyInfo[] _officeKeys = new OfficeKeyInfo[]
    {
        new OfficeKeyInfo("2024", "Office16", "FCPNP-2RG2W-KVY7P-JC6WB-V2RJ7", "Pro Plus 2024"),
        new OfficeKeyInfo("2024", "Office16", "PMBGM-WJFPK-8V7Q2-CW2VJ-Y8PV8", "Standard 2024"),
        new OfficeKeyInfo("2024", "Office16", "S2VYD-7Q2JC-KFJ7C-RQ3KC-H3D38", "Project Pro 2024"),
        new OfficeKeyInfo("2024", "Office16", "T4YW2-W7C7G-YWKGK-JYV9Q-62K3H", "Project Std 2024"),
        new OfficeKeyInfo("2024", "Office16", "BMP65-9GH7R-424RH-BP39C-JB38J", "Visio Pro 2024"),
        new OfficeKeyInfo("2024", "Office16", "3GQXQ-GG9BM-7P7JW-RK82G-YCR7V", "Visio Std 2024"),
        new OfficeKeyInfo("2021", "Office16", "FXYTK-NJJ8C-GB6DW-3DYQT-6F7TH", "Pro Plus 2021"),
        new OfficeKeyInfo("2021", "Office16", "KDX7X-BNVR8-T6G6K-2B4C9-6PTCT", "Standard 2021"),
        new OfficeKeyInfo("2021", "Office16", "FTN22-HG6PQ-RV7QC-GMHHP-FKGDP", "Project Pro 2021"),
        new OfficeKeyInfo("2021", "Office16", "JDW8Q-N4PJ3-DHKCF-Y2HQJ-TV3BM", "Project Std 2021"),
        new OfficeKeyInfo("2021", "Office16", "NHVXN-XXC38-V4QHQ-Q3CQC-9VHXM", "Visio Pro 2021"),
        new OfficeKeyInfo("2021", "Office16", "RJ3CB-DWMY3-WHJCP-B7JGM-Y2VJ8", "Visio Std 2021"),
        new OfficeKeyInfo("2019", "Office16", "NMMKJ-6RK4F-KMJVX-8D9MJ-6MWKP", "Pro Plus 2019"),
        new OfficeKeyInfo("2019", "Office16", "6NWWJ-YQWMR-QKGCB-6TMB3-9D9HK", "Standard 2019"),
        new OfficeKeyInfo("2019", "Office16", "B4NPR-3FKK7-T2MBV-FRQ4W-PKD2B", "Project Pro 2019"),
        new OfficeKeyInfo("2019", "Office16", "C4F7P-NCP8C-6CQPT-MQHV9-JXD2M", "Project Std 2019"),
        new OfficeKeyInfo("2019", "Office16", "9BGNQ-K37YR-RQHF2-38RQ3-7VCBB", "Visio Pro 2019"),
        new OfficeKeyInfo("2019", "Office16", "7TQNQ-K3YQQ-3PFH7-CCPPM-X4VQ2", "Visio Std 2019"),
        new OfficeKeyInfo("M365", "Office16", "HFTND-W9MK4-8B7MJ-B6C4G-XQBR2", "Mondo / M365 Apps"),
        new OfficeKeyInfo("2016", "Office16", "XQNVK-8JYDB-WJ9W3-YJ8YR-WFG99", "Pro Plus 2016"),
        new OfficeKeyInfo("2016", "Office16", "JNRGM-WHDWX-FJJG3-K47QV-DRTFM", "Standard 2016"),
        new OfficeKeyInfo("2016", "Office16", "WGT24-HCNMF-FQ7XH-6M8K7-DRTW9", "Project Pro 2016"),
        new OfficeKeyInfo("2016", "Office16", "D8NRQ-JTYM3-7J2DX-646CT-6836M", "Project Std 2016"),
        new OfficeKeyInfo("2016", "Office16", "69WXN-MBYV6-22PQG-3WGHK-RM6XC", "Visio Pro 2016"),
        new OfficeKeyInfo("2016", "Office16", "NY48V-PPYYH-3F4PX-XJRKJ-W4423", "Visio Std 2016"),
        new OfficeKeyInfo("2013", "Office15", "YC7DK-G2NP3-2QQC3-J6H88-GVGXT", "Pro Plus 2013"),
        new OfficeKeyInfo("2013", "Office15", "KBKQT-2NMXY-JJWGP-M62JB-92CD4", "Standard 2013"),
        new OfficeKeyInfo("2013", "Office15", "FN8TT-7WMH6-2D4X9-M337T-2342K", "Project Pro 2013"),
        new OfficeKeyInfo("2013", "Office15", "6NTH3-CW976-3G3Y2-JK3TX-8QHTT", "Project Std 2013"),
        new OfficeKeyInfo("2013", "Office15", "C2FG9-N6J68-H8BTJ-BW3QX-RM3B3", "Visio Pro 2013"),
        new OfficeKeyInfo("2013", "Office15", "J484Y-4NKBF-W2HMG-DBMJC-PGWR7", "Visio Std 2013"),
        new OfficeKeyInfo("2010", "Office14", "VYBBJ-TRJPB-QFQRF-QFT4D-H3GVB", "Pro Plus 2010"),
        new OfficeKeyInfo("2010", "Office14", "V7QKV-4XVVR-XYV4D-F7DFM-8R6BM", "Standard 2010"),
        new OfficeKeyInfo("2010", "Office14", "YGX6F-PGV49-PGW3J-9BTGG-VHKC6", "Project Pro 2010"),
        new OfficeKeyInfo("2010", "Office14", "4HP3K-88W3F-W2K3D-6677X-F9PGB", "Project Std 2010"),
        new OfficeKeyInfo("2010", "Office14", "7MCW8-VRQVK-G677T-PDJCM-Q8TCP", "Visio Pro 2010"),
        new OfficeKeyInfo("2010", "Office14", "767HD-QGMWX-8QTDB-9G3R2-KHFGJ", "Visio Std 2010"),
    };

    static string FindOspp(string dir)
    {
        string[] bases = { Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
                           Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86) };
        foreach (string b in bases)
        {
            if (string.IsNullOrEmpty(b)) continue;
            string path = string.Format("{0}\\Microsoft Office\\{1}\\OSPP.VBS", b, dir);
            if (File.Exists(path)) return path;
        }
        return null;
    }

    // Detect Office products from registry (all license types)
    struct DetectedOffice
    {
        public string VersionNum;  // "12.0", "14.0", "15.0", "16.0"
        public string ProductName; // e.g. "Microsoft Office Professional Plus 2019"
        public string LicenseType; // "Retail", "Volume", "OEM", "Unknown"
        public string OsppPath;    // path to ospp.vbs if volume licensed
        public bool IsLicensed;    // activation status from registry / ospp
        public DetectedOffice(string vn, string pn, string lt, string ospp)
        {
            VersionNum = vn; ProductName = pn; LicenseType = lt;
            OsppPath = ospp; IsLicensed = false;
        }
    }

    static string RegGetStr(string keyPath, string valueName)
    {
        try { object val = Registry.GetValue(keyPath, valueName, ""); return val == null ? "" : val.ToString(); }
        catch { return ""; }
    }

    static string DetectLicenseChannel(string verNum)
    {
        // Check registration subkeys for license type hints
        string[] roots = { @"HKEY_LOCAL_MACHINE\Software\Microsoft\Office\" + verNum + @"\Registration",
                           @"HKEY_LOCAL_MACHINE\Software\Wow6432Node\Microsoft\Office\" + verNum + @"\Registration" };
        foreach (string root in roots)
        {
            try
            {
                string cleanPath = root.StartsWith("HKEY_LOCAL_MACHINE")
                    ? root.Replace("HKEY_LOCAL_MACHINE", "").TrimStart('\\')
                    : root;
                using (RegistryKey rk = Registry.LocalMachine.OpenSubKey(cleanPath))
                {
                    if (rk == null) continue;
                    foreach (string sub in rk.GetSubKeyNames())
                    {
                        using (RegistryKey sk = rk.OpenSubKey(sub))
                        {
                            if (sk == null) continue;
                            string pid = SafeString(sk.GetValue("ProductID"));
                            if (pid.IndexOf("VOLUME", StringComparison.OrdinalIgnoreCase) >= 0)
                                return "Volume";
                            string channel = SafeString(sk.GetValue("LicenseChannel"));
                            if (channel.IndexOf("VOLUME", StringComparison.OrdinalIgnoreCase) >= 0)
                                return "Volume";
                            string digitalPID = SafeString(sk.GetValue("DigitalProductID"));
                            // Check if ospp.vbs exists for this version (strong indicator of volume)
                            string[] dirs = { "Office16", "Office15", "Office14" };
                            foreach (string d in dirs)
                                if (FindOspp(d) != null) return "Volume";
                        }
                    }
                }
            }
            catch { }
        }
        // Check ClickToRun
        string c2rClient = RegGetStr(@"HKEY_LOCAL_MACHINE\Software\Microsoft\Office\ClickToRun\Configuration", "ClientType");
        if (c2rClient == "VOLUME") return "Volume";
        // Check if ospp.vbs exists
        string[] dirs2 = { "Office16", "Office15", "Office14" };
        foreach (string d in dirs2)
            if (FindOspp(d) != null) return "Volume";
        // If PID contains OEM
        foreach (string root in roots)
        {
            try
            {
                string cleanPath = root.StartsWith("HKEY_LOCAL_MACHINE")
                    ? root.Replace("HKEY_LOCAL_MACHINE", "").TrimStart('\\')
                    : root;
                using (RegistryKey rk = Registry.LocalMachine.OpenSubKey(cleanPath))
                {
                    if (rk == null) continue;
                    foreach (string sub in rk.GetSubKeyNames())
                    {
                        using (RegistryKey sk = rk.OpenSubKey(sub))
                        {
                            if (sk == null) continue;
                            string pid = SafeString(sk.GetValue("ProductID"));
                            if (pid.IndexOf("OEM", StringComparison.OrdinalIgnoreCase) >= 0)
                                return "OEM";
                        }
                    }
                }
            }
            catch { }
        }
        return "Retail";
    }

    static bool IsNoiseProduct(string name)
    {
        string low = name.ToLowerInvariant();
        if (low.IndexOf("language pack") >= 0) return true;
        if (low.IndexOf("proofing") >= 0) return true;
        if (low.IndexOf("office online server") >= 0) return true;
        if (low.IndexOf("visio language pack") >= 0) return true;
        if (low.IndexOf("project language pack") >= 0) return true;
        if (low.IndexOf("office language pack") >= 0) return true;
        if (low.IndexOf("skype for business basic") >= 0) return true;
        if (low.IndexOf("skype for business vdi") >= 0) return true;
        if (low.IndexOf("access runtime") >= 0) return true;
        return false;
    }

    static List<DetectedOffice> ScanAllOffice()
    {
        List<DetectedOffice> list = new List<DetectedOffice>();
        // C2R: check registration paths + WMI for license status
        bool[] c2rSuiteLicensed = new bool[4]; // 0=ProPlus, 1=Project, 2=Visio, 3=Business
        bool[] c2rSuiteHasReg = new bool[4];   // whether registry has an entry at all
        string[] c2rRootsCheck = {
            @"HKEY_LOCAL_MACHINE\Software\Microsoft\Office\ClickToRun\REGISTRY\MACHINE\Software\Microsoft\Office\16.0\Registration",
            @"HKEY_LOCAL_MACHINE\Software\Microsoft\Office\ClickToRun\REGISTRY\MACHINE\Software\Wow6432Node\Microsoft\Office\16.0\Registration"
        };
        foreach (string cr in c2rRootsCheck)
        {
            try
            {
                string cp = cr.StartsWith("HKEY_LOCAL_MACHINE") ? cr.Replace("HKEY_LOCAL_MACHINE", "").TrimStart('\\') : cr;
                using (RegistryKey rk = Registry.LocalMachine.OpenSubKey(cp))
                {
                    if (rk == null) continue;
                    foreach (string sub in rk.GetSubKeyNames())
                    {
                        using (RegistryKey sk = rk.OpenSubKey(sub))
                        {
                            if (sk == null) continue;
                            string pn2 = SafeString(sk.GetValue("ProductName"));
                            if (string.IsNullOrEmpty(pn2)) continue;
                            object ls2 = sk.GetValue("LicenseStatus");
                            int lsVal = -1;
                            if (ls2 != null) { try { lsVal = Convert.ToInt32(ls2); } catch { } }
                            bool lic = (lsVal == 1);

                            int idx = -1;
                            if (pn2.IndexOf("ProPlus", StringComparison.OrdinalIgnoreCase) >= 0 ||
                                pn2.IndexOf("专业增强版", StringComparison.OrdinalIgnoreCase) >= 0 ||
                                pn2.IndexOf("企业应用版", StringComparison.OrdinalIgnoreCase) >= 0) idx = 0;
                            else if (pn2.IndexOf("Project", StringComparison.OrdinalIgnoreCase) >= 0) idx = 1;
                            else if (pn2.IndexOf("Visio", StringComparison.OrdinalIgnoreCase) >= 0) idx = 2;
                            else if (pn2.IndexOf("Business", StringComparison.OrdinalIgnoreCase) >= 0 ||
                                     pn2.IndexOf("小型企业", StringComparison.OrdinalIgnoreCase) >= 0) idx = 3;
                            if (idx >= 0)
                            {
                                // Only update licensed if we find a real LicenseStatus value
                                if (ls2 != null)
                                {
                                    c2rSuiteHasReg[idx] = true;
                                    if (lic) c2rSuiteLicensed[idx] = true;
                                }
                            }
                        }
                    }
                }
            }
            catch { }
        }

        // WMI fallback for Office licensing status
        try
        {
            using (var wmi = new ManagementObjectSearcher(
                "SELECT LicenseStatus, Description FROM SoftwareLicensingProduct " +
                "WHERE Description LIKE '%Office%' OR Description LIKE '%Microsoft 365%'"))
            {
                foreach (var w in wmi.Get())
                {
                    int wmiLic = SafeInt(w["LicenseStatus"]);
                    string wmiDesc = SafeString(w["Description"]);
                    bool wmiAct = (wmiLic == 1);
                    if (wmiAct)
                    {
                        if (wmiDesc.IndexOf("ProPlus", StringComparison.OrdinalIgnoreCase) >= 0 ||
                            wmiDesc.IndexOf("Office", StringComparison.OrdinalIgnoreCase) >= 0 ||
                            wmiDesc.IndexOf("Mondo", StringComparison.OrdinalIgnoreCase) >= 0)
                            c2rSuiteLicensed[0] = true;
                        if (wmiDesc.IndexOf("Project", StringComparison.OrdinalIgnoreCase) >= 0)
                            c2rSuiteLicensed[1] = true;
                        if (wmiDesc.IndexOf("Visio", StringComparison.OrdinalIgnoreCase) >= 0)
                            c2rSuiteLicensed[2] = true;
                        if (wmiDesc.IndexOf("Business", StringComparison.OrdinalIgnoreCase) >= 0)
                            c2rSuiteLicensed[3] = true;
                    }
                }
            }
        }
        catch { }

        // C2R: use ProductReleaseIds for suite-level product listing
        string c2rProducts = RegGetStr(@"HKEY_LOCAL_MACHINE\Software\Microsoft\Office\ClickToRun\Configuration", "ProductReleaseIds");
        string c2rClient = RegGetStr(@"HKEY_LOCAL_MACHINE\Software\Microsoft\Office\ClickToRun\Configuration", "ClientType");

        if (!string.IsNullOrEmpty(c2rProducts))
        {
            string lt = (c2rClient == "VOLUME") ? "Volume" : "Retail";
            string[] products = c2rProducts.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string p in products)
            {
                string name = p.Trim();
                bool licensed = false;
                int idx = -1;
                if (name.IndexOf("ProPlus", StringComparison.OrdinalIgnoreCase) >= 0)
                { name = "Microsoft 365 企业应用版 (ProPlus)"; licensed = c2rSuiteLicensed[0]; idx = 0; }
                else if (name.IndexOf("Business", StringComparison.OrdinalIgnoreCase) >= 0)
                { name = "Microsoft 365 商业版"; licensed = c2rSuiteLicensed[3]; idx = 3; }
                else if (name.IndexOf("Standard", StringComparison.OrdinalIgnoreCase) >= 0)
                    name = "Microsoft 365 标准版";
                else if (name.IndexOf("Project", StringComparison.OrdinalIgnoreCase) >= 0)
                { name = "Microsoft Project " + (name.IndexOf("Pro", StringComparison.OrdinalIgnoreCase) >= 0 ? "专业版" : "标准版"); licensed = c2rSuiteLicensed[1]; idx = 1; }
                else if (name.IndexOf("Visio", StringComparison.OrdinalIgnoreCase) >= 0)
                { name = "Microsoft Visio " + (name.IndexOf("Pro", StringComparison.OrdinalIgnoreCase) >= 0 ? "专业版" : "标准版"); licensed = c2rSuiteLicensed[2]; idx = 2; }

                // C2R Retail subscription: no LicenseStatus in registry → assume activated via sign-in
                if (lt == "Retail" && !licensed && idx >= 0 && !c2rSuiteHasReg[idx])
                    licensed = true;

                string ospp = (lt == "Volume") ? FindOspp("Office16") : null;
                DetectedOffice d = new DetectedOffice("16.0", name, lt, ospp);
                d.IsLicensed = licensed;
                list.Add(d);
            }
        }

        // MSI-based detection (Office 2007-2019 MSI)
        string[][] verMap = new string[][] {
            new string[] { "12.0", "2007" },
            new string[] { "14.0", "2010" },
            new string[] { "15.0", "2013" },
            new string[] { "16.0", "2016+" }
        };

        foreach (var vm in verMap)
        {
            string ver = vm[0];
            string[] roots = { @"HKEY_LOCAL_MACHINE\Software\Microsoft\Office\" + ver + @"\Registration",
                               @"HKEY_LOCAL_MACHINE\Software\Wow6432Node\Microsoft\Office\" + ver + @"\Registration" };
            foreach (string root in roots)
            {
                try
                {
                    string cleanPath = root.StartsWith("HKEY_LOCAL_MACHINE")
                        ? root.Replace("HKEY_LOCAL_MACHINE", "").TrimStart('\\')
                        : root;
                    using (RegistryKey rk = Registry.LocalMachine.OpenSubKey(cleanPath))
                    {
                        if (rk == null) continue;
                        foreach (string sub in rk.GetSubKeyNames())
                        {
                            using (RegistryKey sk = rk.OpenSubKey(sub))
                            {
                                if (sk == null) continue;
                                string pn = SafeString(sk.GetValue("ProductName"));
                                if (string.IsNullOrEmpty(pn))
                                    pn = SafeString(sk.GetValue("DisplayName"));
                                if (string.IsNullOrEmpty(pn)) continue;
                                // Skip individual app entries (Word/Excel/etc.) - only show suites
                                if (IsNoiseProduct(pn)) continue;
                                if (pn.IndexOf("Microsoft Office", StringComparison.OrdinalIgnoreCase) < 0 &&
                                    pn.IndexOf("Microsoft 365", StringComparison.OrdinalIgnoreCase) < 0 &&
                                    pn.IndexOf("Microsoft Project", StringComparison.OrdinalIgnoreCase) < 0 &&
                                    pn.IndexOf("Microsoft Visio", StringComparison.OrdinalIgnoreCase) < 0 &&
                                    pn.IndexOf("Skype for Business", StringComparison.OrdinalIgnoreCase) < 0)
                                    continue;

                                // Deduplicate: skip if we already have this product
                                bool dup = false;
                                foreach (var existing in list)
                                    if (existing.ProductName == pn) { dup = true; break; }
                                if (dup) continue;

                                string lt = "Retail";
                                string pid = SafeString(sk.GetValue("ProductID"));
                                if (pid.IndexOf("VOLUME", StringComparison.OrdinalIgnoreCase) >= 0) lt = "Volume";
                                else if (pid.IndexOf("OEM", StringComparison.OrdinalIgnoreCase) >= 0) lt = "OEM";

                                object lsObj = sk.GetValue("LicenseStatus");
                                bool licensed = false;
                                if (lsObj != null) { try { licensed = (Convert.ToInt32(lsObj) == 1); } catch { } }

                                string ospp = (lt == "Volume") ? FindOspp("Office16") ?? FindOspp("Office15") ?? FindOspp("Office14") : null;
                                DetectedOffice detected = new DetectedOffice(ver, pn, lt, ospp);
                                detected.IsLicensed = licensed;
                                list.Add(detected);
                            }
                        }
                    }
                }
                catch { }
            }
        }

        return list;
    }

    static string GetOfficeStatusFromOspp(string osppPath, string productName)
    {
        string output = Run("cscript", string.Format("//nologo \"{0}\" /dstatusall", osppPath));
        // Parse output for product status
        string[] lines = output.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
        bool inProduct = false;
        string currentProduct = "";
        string currentStatus = "";
        foreach (string line in lines)
        {
            string t = line.Trim();
            if (t.StartsWith("Product name:", StringComparison.OrdinalIgnoreCase))
            {
                currentProduct = t.Substring("Product name:".Length).Trim();
                inProduct = true;
                currentStatus = "";
            }
            else if (inProduct && t.StartsWith("Licensed:", StringComparison.OrdinalIgnoreCase))
            {
                currentStatus = t.Substring("Licensed:".Length).Trim();
                if (currentProduct.IndexOf(productName, StringComparison.OrdinalIgnoreCase) >= 0)
                    return currentStatus;
            }
        }
        return "";
    }

    // Helper: install GVLK + set KMS + activate Office via ospp.vbs
    static void ActivateOfficeWithKms(string ospp, string targetVer, string productName)
    {
        if (string.IsNullOrEmpty(ospp))
        {
            Console.WriteLine("  找不到 OSPP.VBS，跳过。");
            return;
        }
        string matchedKey = null;
        string matchedLabel = null;
        string pnLower = productName.ToLowerInvariant();
        foreach (var ok in _officeKeys)
        {
            if (pnLower.Contains("pro") && pnLower.Contains("plus") &&
                ok.ProductLabel.ToLowerInvariant().Contains("pro plus"))
            { matchedKey = ok.Key; matchedLabel = ok.ProductLabel; break; }
            if (pnLower.Contains("standard") &&
                ok.ProductLabel.ToLowerInvariant().Contains("standard"))
            { matchedKey = ok.Key; matchedLabel = ok.ProductLabel; break; }
            if (pnLower.Contains("project") &&
                ok.ProductLabel.ToLowerInvariant().Contains("project"))
            { matchedKey = ok.Key; matchedLabel = ok.ProductLabel; break; }
            if (pnLower.Contains("visio") &&
                ok.ProductLabel.ToLowerInvariant().Contains("visio"))
            { matchedKey = ok.Key; matchedLabel = ok.ProductLabel; break; }
        }
        if (matchedKey == null)
        {
            Console.WriteLine("  选择要激活的产品:");
            int idx = 0;
            int[] optIdx = new int[_officeKeys.Length];
            string tv = targetVer;
            if (tv == "2016+") tv = "2016";
            for (int i = 0; i < _officeKeys.Length; i++)
            {
                bool match = false;
                if (_officeKeys[i].OsppDir == "Office16" &&
                    (tv == "2016" || tv == "2019" || tv == "2021" || tv == "2024" || tv == "M365" || tv == "2016+"))
                    match = true;
                else if (_officeKeys[i].OsppDir == "Office15" && tv == "2013")
                    match = true;
                else if (_officeKeys[i].OsppDir == "Office14" && tv == "2010")
                    match = true;
                if (match)
                {
                    idx++; optIdx[idx - 1] = i;
                    Console.WriteLine("    " + idx + ". " + _officeKeys[i].ProductLabel);
                }
            }
            if (idx == 0) { Console.WriteLine("  没有匹配的密钥，跳过。"); return; }
            Console.Write("  请选择 (1-" + idx + "，默认 1): ");
            string choice = Console.ReadLine();
            int sel = 1;
            int.TryParse(choice, out sel);
            if (sel < 1 || sel > idx) sel = 1;
            matchedKey = _officeKeys[optIdx[sel - 1]].Key;
            matchedLabel = _officeKeys[optIdx[sel - 1]].ProductLabel;
        }
        Console.WriteLine("  正在安装密钥: " + matchedLabel);
        Console.WriteLine(Run("cscript", string.Format("//nologo \"{0}\" /inpkey:{1}", ospp, matchedKey)));
        Console.WriteLine("  正在设置 KMS 服务器: sohai.space");
        Console.WriteLine(Run("cscript", string.Format("//nologo \"{0}\" /sethst:sohai.space", ospp)));
        Console.WriteLine("  正在激活...");
        Console.WriteLine(Run("cscript", string.Format("//nologo \"{0}\" /act", ospp)));
    }

    static List<DetectedOffice> ProcessOffice()
    {
        Console.WriteLine("\n========================================");
        Console.WriteLine("  Office 检测与激活");
        Console.WriteLine("========================================");

        List<DetectedOffice> offices = ScanAllOffice();

        if (offices.Count == 0)
        {
            Console.WriteLine("  未检测到 Microsoft Office 安装。");
            return offices;
        }

        for (int oi = 0; oi < offices.Count; oi++)
        {
            DetectedOffice off = offices[oi];
            bool licensed = off.IsLicensed;

            if (off.LicenseType == "Volume" && !string.IsNullOrEmpty(off.OsppPath))
            {
                string osppStatus = GetOfficeStatusFromOspp(off.OsppPath, off.ProductName);
                if (!string.IsNullOrEmpty(osppStatus))
                {
                    if (osppStatus.IndexOf("Licensed", StringComparison.OrdinalIgnoreCase) >= 0 ||
                        osppStatus.IndexOf("1", StringComparison.OrdinalIgnoreCase) >= 0)
                        licensed = true;
                }
            }

            DetectedOffice updated = new DetectedOffice(off.VersionNum, off.ProductName, off.LicenseType, off.OsppPath);
            updated.IsLicensed = licensed;
            offices[oi] = updated;
        }

        // Display summary
        for (int oi = 0; oi < offices.Count; oi++)
        {
            DetectedOffice off = offices[oi];
            string pn = off.ProductName;
            Console.WriteLine("\n  产品: " + pn);
            Console.WriteLine("  授权类型: " + off.LicenseType);
            Console.WriteLine("  状态:  " + (off.IsLicensed ? "已激活" : "未激活"));
        }
        return offices;
    }

    // ============== Main ==============

    static int Main()
    {
        try
        {
            using (var identity = System.Security.Principal.WindowsIdentity.GetCurrent())
            {
                var principal = new System.Security.Principal.WindowsPrincipal(identity);
                if (!principal.IsInRole(System.Security.Principal.WindowsBuiltInRole.Administrator))
                {
                    Console.WriteLine("请以管理员身份运行。");
                    Console.WriteLine("按任意键退出...");
                    Console.ReadKey();
                    return 1;
                }
            }

            string caption = "";
            using (var searcher = new ManagementObjectSearcher("SELECT Caption FROM Win32_OperatingSystem"))
            {
                foreach (var o in searcher.Get())
                    caption = o["Caption"] == null ? "" : o["Caption"].ToString();
            }

            if (string.IsNullOrEmpty(caption))
            {
                Console.WriteLine("无法检测操作系统。");
                Console.WriteLine("按任意键退出...");
                Console.ReadKey();
                return 1;
            }

            string key = GetKey(caption);
            if (key == null)
            {
                Console.WriteLine("无法识别的版本: " + caption);
                Console.WriteLine("按任意键退出...");
                Console.ReadKey();
                return 1;
            }

            // Check activation status
            string statusText = "未知";
            string activationType = "未知";
            int graceDays = 0;
            int winLicenseStatus = -1;
            using (var searcher = new ManagementObjectSearcher(
                "SELECT LicenseStatus, GracePeriodRemaining, ProductKeyChannel, " +
                "VLActivationType, KeyManagementServiceMachine, Description, ProductKeyID2 " +
                "FROM SoftwareLicensingProduct " +
                "WHERE PartialProductKey IS NOT NULL AND ApplicationID = '55c92734-d682-4d71-983e-d6ec3f16059f'"))
            {
                foreach (var o in searcher.Get())
                {
                    int ls = SafeInt(o["LicenseStatus"]);
                    winLicenseStatus = ls;
                    object g = o["GracePeriodRemaining"];
                    graceDays = (g == null || g is DBNull) ? 0 : SafeInt(g) / 1440;
                    switch (ls)
                    {
                        case 0: statusText = "未授权"; break;
                        case 1: statusText = "已激活"; break;
                        case 2: statusText = "宽限期（剩余" + graceDays + "天）"; break;
                        case 3: statusText = "OOT 宽限期（" + graceDays + "天）"; break;
                        case 4: statusText = "非正版宽限期（" + graceDays + "天）"; break;
                        case 5: statusText = "通知"; break;
                        case 6: statusText = "扩展宽限期（" + graceDays + "天）"; break;
                    }

                    string channel = SafeString(o["ProductKeyChannel"]);
                    int vlType = SafeInt(o["VLActivationType"]);
                    string kmsServer = SafeString(o["KeyManagementServiceMachine"]);

                    // Check for digital license
                    bool hasDigitalLicense = false;
                    string desc = SafeString(o["Description"]);
                    string pkid2 = SafeString(o["ProductKeyID2"]);
                    if (!string.IsNullOrEmpty(pkid2))
                        hasDigitalLicense = true;
                    if (desc.IndexOf("Digital", StringComparison.OrdinalIgnoreCase) >= 0 ||
                        desc.IndexOf("数字", StringComparison.OrdinalIgnoreCase) >= 0)
                        hasDigitalLicense = true;

                    if (channel == "Volume:GVLK")
                    {
                        if (vlType == 1 || !string.IsNullOrEmpty(kmsServer))
                            activationType = "KMS 激活";
                        else if (vlType == 2)
                            activationType = "MAK 激活（KMS 客户端密钥）";
                        else if (vlType == 4)
                            activationType = "AD 激活（KMS 客户端密钥）";
                        else
                            activationType = "批量 GVLK（未激活）";
                    }
                    else if (channel == "Volume:MAK")
                        activationType = "MAK 激活";
                    else if (channel.IndexOf("Retail", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        if (hasDigitalLicense)
                            activationType = "数字许可证激活";
                        else
                            activationType = "零售激活";
                    }
                    else if (channel.IndexOf("OEM", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        if (channel.IndexOf("SLP", StringComparison.OrdinalIgnoreCase) >= 0 ||
                            channel.IndexOf("SLIC", StringComparison.OrdinalIgnoreCase) >= 0 ||
                            channel.IndexOf("SLC", StringComparison.OrdinalIgnoreCase) >= 0)
                            activationType = "OEM SLIC 激活（BIOS）";
                        else
                            activationType = "OEM 激活";
                    }
                    else if (!string.IsNullOrEmpty(kmsServer))
                        activationType = "KMS 激活（" + kmsServer + "）";
                    else if (channel != "")
                        activationType = channel;
                    else if (ls == 1)
                        activationType = "未知（已激活）";
                    else
                        activationType = "未知";

                    break;
                }
            }

            string sysDir = Environment.SystemDirectory;
            string slmgr = string.Format("\"{0}\\slmgr.vbs\"", sysDir);

            Console.WriteLine("========================================");
            Console.WriteLine("  KMS 激活工具 - sohai.space");
            Console.WriteLine("========================================");
            Console.WriteLine("  系统:  " + caption);
            Console.WriteLine("  状态:  " + statusText);
            Console.WriteLine("  类型:  " + activationType);
            Console.WriteLine("  密钥:  " + key);
            Console.WriteLine("========================================");

            bool winActivated = (winLicenseStatus == 1) || statusText.IndexOf("已激活", StringComparison.OrdinalIgnoreCase) >= 0;

            // Office section (display only)
            List<DetectedOffice> officeList = ProcessOffice();

            // Determine if any action is needed
            bool winNeedsWork = !winActivated;
            bool officeVolumeNeedsWork = false;
            bool officeRetailExists = false;
            foreach (var o in officeList)
            {
                if (o.LicenseType == "Retail") officeRetailExists = true;
                if (o.LicenseType == "Volume" && !o.IsLicensed) officeVolumeNeedsWork = true;
                if (o.LicenseType == "Retail" && !o.IsLicensed) officeVolumeNeedsWork = true;
            }

            if (!winNeedsWork && !officeVolumeNeedsWork && !officeRetailExists)
            {
                Console.WriteLine("\n  所有产品均已激活。");
            }
            else
            {
                // Show menu
                bool menuActive = true;
                while (menuActive)
                {
                    Console.WriteLine("\n==============================");
                    Console.WriteLine("  请选择操作:");
                    Console.WriteLine("==============================");
                    Console.WriteLine("  1. 激活 Windows（安装 GVLK + KMS 激活）");
                    Console.WriteLine("  2. 仅安装 Windows GVLK 密钥");
                    if (officeVolumeNeedsWork || officeRetailExists)
                        Console.WriteLine("  3. 激活 Office（批量版）");
                    if (officeRetailExists)
                        Console.WriteLine("  4. 将零售版 Office 转换为批量授权版");
                    Console.WriteLine("  Q. 退出");
                    Console.Write("  请选择: ");
                    string menuChoice = Console.ReadLine();
                    if (string.IsNullOrEmpty(menuChoice)) continue;
                    string mc = menuChoice.ToUpper();

                    if (mc == "Q") { menuActive = false; break; }

                    else if (mc == "1")
                    {
                        if (winActivated)
                        {
                            Console.WriteLine("Windows 已激活，无需重复操作。");
                            Console.WriteLine("\n按任意键继续...");
                            Console.ReadKey();
                            continue;
                        }
                        Console.WriteLine("正在卸载旧产品密钥...");
                        Console.WriteLine(Run("cscript", string.Format("//nologo {0} /cpky", slmgr)));
                        Console.WriteLine(Run("cscript", string.Format("//nologo {0} /upk", slmgr)));
                        Console.WriteLine("正在安装 GVLK 产品密钥...");
                        Console.WriteLine(Run("cscript", string.Format("//nologo {0} /ipk {1}", slmgr, key)));
                        Console.WriteLine("正在设置 KMS 服务器: sohai.space");
                        Console.WriteLine(Run("cscript", string.Format("//nologo {0} /skms sohai.space", slmgr)));
                        Console.WriteLine("正在激活...");
                        Console.WriteLine(Run("cscript", string.Format("//nologo {0} /ato", slmgr)));
                        Console.WriteLine(Run("cscript", string.Format("//nologo {0} /dli", slmgr)));
                        Console.WriteLine("\n按任意键继续...");
                        Console.ReadKey();
                    }
                    else if (mc == "2")
                    {
                        if (winActivated)
                        {
                            Console.WriteLine("Windows 已激活，无需重复安装密钥。");
                            Console.WriteLine("\n按任意键继续...");
                            Console.ReadKey();
                            continue;
                        }
                        Console.WriteLine("正在卸载旧产品密钥...");
                        Console.WriteLine(Run("cscript", string.Format("//nologo {0} /cpky", slmgr)));
                        Console.WriteLine(Run("cscript", string.Format("//nologo {0} /upk", slmgr)));
                        Console.WriteLine("正在安装 GVLK 产品密钥...");
                        Console.WriteLine(Run("cscript", string.Format("//nologo {0} /ipk {1}", slmgr, key)));
                        Console.WriteLine("  完成，请使用选项 1 激活，或手动运行 slmgr /ato。");
                        Console.WriteLine("\n按任意键继续...");
                        Console.ReadKey();
                    }
                    else if (mc == "3" && (officeVolumeNeedsWork || officeRetailExists))
                    {
                        foreach (var o in officeList)
                        {
                            if (o.LicenseType == "Volume" && !o.IsLicensed)
                            {
                                string verLabel = GetOfficeVerLabel(o.ProductName, o.VersionNum);
                                Console.WriteLine("\n  正在激活: " + o.ProductName);
                                string ospp = o.OsppPath ?? FindOspp("Office16") ?? FindOspp("Office15") ?? FindOspp("Office14");
                                ActivateOfficeWithKms(ospp, verLabel, o.ProductName);
                            }
                        }
                        bool hasUnactivatedRetail = false;
                        foreach (var o in officeList)
                            if (o.LicenseType == "Retail" && !o.IsLicensed) hasUnactivatedRetail = true;
                        if (hasUnactivatedRetail)
                        {
                            Console.WriteLine("\n  检测到零售版 Office 产品，请使用选项 4 转换。");
                        }
                        Console.WriteLine("\n按任意键继续...");
                        Console.ReadKey();
                    }
                    else if (mc == "4" && officeRetailExists)
                    {
                        foreach (var o in officeList)
                        {
                            if (o.LicenseType == "Retail")
                            {
                                string verLabel = GetOfficeVerLabel(o.ProductName, o.VersionNum);
                                Console.WriteLine("\n  正在转换: " + o.ProductName);
                                string ospp = FindOspp("Office16") ?? FindOspp("Office15") ?? FindOspp("Office14");
                                if (string.IsNullOrEmpty(ospp))
                                    Console.WriteLine("  找不到 OSPP.VBS，无法转换 " + o.ProductName);
                                else
                                    ActivateOfficeWithKms(ospp, verLabel, o.ProductName);
                            }
                        }
                        Console.WriteLine("\n按任意键继续...");
                        Console.ReadKey();
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("错误: " + ex.Message);
        }

        Console.WriteLine("\n按任意键退出...");
        Console.ReadKey();
        return 0;
    }

    static string GetOfficeVerLabel(string productName, string versionNum)
    {
        if (versionNum == "14.0") return "2010";
        if (versionNum == "15.0") return "2013";
        if (productName.IndexOf("2024") >= 0) return "2024";
        if (productName.IndexOf("2021") >= 0) return "2021";
        if (productName.IndexOf("2019") >= 0) return "2019";
        if (productName.IndexOf("2016") >= 0) return "2016";
        if (productName.IndexOf("2013") >= 0) return "2013";
        if (productName.IndexOf("2010") >= 0) return "2010";
        if (productName.IndexOf("2007") >= 0) return "2007";
        return "2016+";
    }
}
