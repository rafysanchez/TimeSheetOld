using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

public partial class _Default : System.Web.UI.Page 
{
    protected void Page_Load(object sender, EventArgs e)
    {        
        if (!IsPostBack)
        {
            this.gvSelecao.DataSource = this.Dados();
            this.gvSelecao.DataBind();
        }
    }

    protected DataTable Dados()
    {
        DataTable tabela = new DataTable();
        DataRow linha = null;
        tabela.Columns.Add("ID");
        tabela.Columns.Add("LETRA");
        tabela.Columns.Add("CODIGO");

        for (int i = 1; i <= 10; i++)
        {
            linha = tabela.NewRow();
            linha["ID"] = (i * 8764);
            byte ascii = byte.Parse((i+65).ToString());
            linha["LETRA"] = Convert.ToChar(ascii);
            linha["CODIGO"] = System.Guid.NewGuid().ToString("b");
            tabela.Rows.Add(linha);
            tabela.AcceptChanges();
        }
        return tabela;
    }
    protected void gvSelecao_PreRender(object sender, EventArgs e)
    {
        GridView gv = (GridView)sender;
        foreach (GridViewRow row in gv.Rows)
            row.Attributes.Add("onclick", "Selecionar(this,'#FFFF00');");

    }
}
