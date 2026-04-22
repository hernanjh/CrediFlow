import { utils, writeFile } from 'xlsx'
import jsPDF from 'jspdf'
import autoTable from 'jspdf-autotable'

export const exportToExcel = (data: any[], filename: string, sheetName: string = 'Datos') => {
  if (!data || data.length === 0) return;
  const worksheet = utils.json_to_sheet(data);
  const workbook = utils.book_new();
  utils.book_append_sheet(workbook, worksheet, sheetName);
  writeFile(workbook, `${filename}.xlsx`);
}

export const exportToPDF = (
  headers: string[], 
  data: any[][], 
  title: string, 
  filename: string, 
  orientation: 'portrait' | 'landscape' = 'landscape'
) => {
  if (!data || data.length === 0) return;
  const doc = new jsPDF(orientation);
  
  // Custom Header
  doc.setFontSize(18);
  doc.setTextColor(40, 40, 40);
  doc.text('CrediFlow - Reporte de Sistema', 14, 20);
  
  doc.setFontSize(12);
  doc.setTextColor(100, 100, 100);
  doc.text(title, 14, 28);
  
  const dateStr = new Date().toLocaleString();
  doc.setFontSize(9);
  doc.text(`Generado: ${dateStr}`, 14, 34);

  // Table
  autoTable(doc, {
    startY: 40,
    head: [headers],
    body: data,
    theme: 'grid',
    headStyles: { fillColor: [59, 130, 246] }, // Azul primary
    styles: { fontSize: 8, cellPadding: 3 },
    alternateRowStyles: { fillColor: [245, 247, 250] } // Light gray
  });

  doc.save(`${filename}.pdf`);
}
