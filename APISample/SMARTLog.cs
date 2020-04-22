using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMARTQuery
{
    class SMARTLog
    {  
        public SMARTItem raw_read_err = new SMARTItem();
        public SMARTItem reallocate_sector_cnt = new SMARTItem();
        public SMARTItem pwr_on_hrs = new SMARTItem();
        public SMARTItem pwr_cycle_cnt = new SMARTItem();
        public SMARTItem uncorrect_sector_cnt = new SMARTItem();
        public SMARTItem vaild_spare = new SMARTItem();
        public SMARTItem init_invalid_blks = new SMARTItem();
        public SMARTItem total_tlc_erase_cnt = new SMARTItem();
        public SMARTItem max_tlc_erase_cnt = new SMARTItem();
        public SMARTItem min_tlc_erase_cnt = new SMARTItem();
        public SMARTItem avg_tlc_erase_cnt = new SMARTItem();
        public SMARTItem rsvd_a8 = new SMARTItem();
        public SMARTItem percent_life_remaning = new SMARTItem();
        public SMARTItem rsvd_af = new SMARTItem();
        public SMARTItem rsvd_b0 = new SMARTItem();
        public SMARTItem rsvd_b1 = new SMARTItem();
        public SMARTItem rsvd_b2 = new SMARTItem();
        public SMARTItem prog_fail_cnt = new SMARTItem();
        public SMARTItem erase_fail_cnt = new SMARTItem();
        public SMARTItem pwr_off_retract_cnt = new SMARTItem();
        public SMARTItem temperature = new SMARTItem();
        public SMARTItem cumulat_ecc_bit_correct_cnt = new SMARTItem();
        public SMARTItem reallocation_event_cnt = new SMARTItem();
        public SMARTItem current_pending_sector_cnt = new SMARTItem();
        public SMARTItem off_line_scan_uncorrect_cnt = new SMARTItem();
        public SMARTItem ultra_dma_crc_err_rate = new SMARTItem();
        public SMARTItem avail_rsv_space = new SMARTItem();
        public SMARTItem total_lba_write = new SMARTItem();
        public SMARTItem total_lba_read = new SMARTItem();
        public SMARTItem cumulat_prog_nand_pg = new SMARTItem();
    }


    public class SMARTLog_NVMe
    {
        public SMARTItem critical_warning = new SMARTItem();
        public SMARTItem temperature = new SMARTItem();
        public SMARTItem avail_spare = new SMARTItem();
        public SMARTItem spare_thresh = new SMARTItem();
        public SMARTItem percent_used = new SMARTItem();
        public SMARTItem endu_grp_crit_warn_sumry = new SMARTItem();
        public SMARTItem rsvd7 = new SMARTItem();
        public SMARTItem data_units_read = new SMARTItem();
        public SMARTItem data_units_written = new SMARTItem();
        public SMARTItem host_reads = new SMARTItem();
        public SMARTItem host_writes = new SMARTItem();
        public SMARTItem ctrl_busy_time = new SMARTItem();
        public SMARTItem power_cycles = new SMARTItem();
        public SMARTItem power_on_hours = new SMARTItem();
        public SMARTItem unsafe_shutdowns = new SMARTItem();
        public SMARTItem media_errors = new SMARTItem();
        public SMARTItem num_err_log_entries = new SMARTItem();
        public SMARTItem warning_temp_time = new SMARTItem();
        public SMARTItem critical_comp_time = new SMARTItem();
    }

    public class SMARTItem 
    {
        public string content { get; set; }
        public string raw { get; set; }
    }
}
