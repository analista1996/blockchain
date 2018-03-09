using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NBitcoin;
using System.Windows.Forms;
using NBitcoin.Stealth;
using NBitcoin.BitcoinCore;
using NBitcoin.Protocol.Payloads;
namespace WD_Easy_Control.classes.BlockChain
{
    public class Transacao
    {
       public Transaction transaction = new Transaction();
       public Block bloco = new Block();
       //gero uma chave privada com os dados fornecidos
       private static byte[] chave = {1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25,26,27,28,29,30,31,32};
       static RandomUtils tan = new RandomUtils();
      
       static Key sistema = new Key(chave);
       static Key sangria = new Key(chave);
       public  BitcoinAddress pass;
       //gero uma chave publica
       static PubKey pubKey = sangria.PubKey;
       static KeyId hash = pubKey.Hash;
       //recuperando o passe bitcoin.
       static Script scrip;
       public void CriaTransacao(decimal valor)
       {
          var scriptPubKey = PayToMultiSigTemplate.Instance.GenerateScriptPubKey(1, new[] { sistema.PubKey, sangria.PubKey });
          scrip = scriptPubKey;
          transaction.Outputs.Add(new TxOut(Money.Coins(valor),  scriptPubKey));
          bloco.AddTransaction(transaction);
       }
      
       public bool RetiraValor(decimal saida)
       {
           bool sucesso = false;
            RandomUtils.Random = new UnsecureRandom();
            //gerador de chave privada.
            Key privateKeyGenerator = new Key();
            BitcoinSecret bitcoinSecretFromPrivateKeyGenerator = privateKeyGenerator.GetBitcoinSecret(Network.Main);
            Key privateKeyFromBitcoinSecret = bitcoinSecretFromPrivateKeyGenerator.PrivateKey;
    
            BitcoinSecret chave_sistema = new BitcoinSecret("L5DZpEdbDDhhk3EqtktmGXKv3L9GxttYTecxDhM5huLd82qd9uvo", Network.Main);
            BitcoinSecret chave_sangria = new BitcoinSecret("KyStsAHgSehHvewS5YfGwhQGfEWYd8qY2XZg6q2M6TqaM8Q8rayg", Network.Main);
           
            Key sangriaPrivateKey = chave_sangria.PrivateKey;

            Key danielKey = chave_sistema.PrivateKey;

            Script ScriptPagamentoSangria = PayToMultiSigTemplate.Instance.GenerateScriptPubKey(1, danielKey.PubKey);

            //esta transação manda o valor para a transação sangria.
            //A coisa que você deve notar é que essa transação é adicionada por vários tipos de scriptPubKey, como P2PK (bobPrivateKey.PubKey), P2PKH (alicePrivateKey.PubKey.Hash) e ScriptPubKey multi-sig (scriptPubKeyOfBobAlice).
            var txPegandoCoins = new Transaction();
            txPegandoCoins.Outputs.Add(new TxOut(Money.Coins(bloco.Transactions[0].TotalOut.ToDecimal(MoneyUnit.BTC)), danielKey.PubKey.Hash));
            //txPegandoCoins.Outputs.Add(new TxOut(Money.Coins(saida), ScriptPagamentoSangria));
            MessageBox.Show(txPegandoCoins.TotalOut.ToString());
            //Agora, digamos que eles querem usar as moedas dessa transação para pagar Satoshi.
            //Primeiro, eles precisam pegar suas moedas.
            Coin[] coins = txPegandoCoins.Outputs.AsCoins().ToArray();

            Coin danielCoin = coins[0];

             //Crie a transação usando os recursos da classe TransactionBuilder.
            var builderForSendingCoinToSatoshi = new TransactionBuilder();
            Transaction txForSpendingCoinToSatoshi = builderForSendingCoinToSatoshi
                    .AddCoins(danielCoin)
                    .AddKeys(danielKey)
                    .Send(sangriaPrivateKey, Money.Coins(saida))
                    .SetChange(danielKey)
                    .SendFees(Money.Coins(0.0001m))
                    .Then().BuildTransaction(sign: true);
            MessageBox.Show( txForSpendingCoinToSatoshi.ToString(),"Informações sobre as transações");

            //Então você pode verificar se está totalmente assinado e pronto para enviar para a rede.
            //Verifique se você não estragou a transação.
            MessageBox.Show("Operação Assinada: "+builderForSendingCoinToSatoshi.Verify(txForSpendingCoinToSatoshi).ToString());
            if (builderForSendingCoinToSatoshi.Verify(txForSpendingCoinToSatoshi) == true)
           {
               sucesso = true;
           }
           //atualizo o valor  na transação
            var valor = txForSpendingCoinToSatoshi.TotalOut.ToDecimal(MoneyUnit.BTC) - saida;
            bloco.Transactions[0].Outputs[0].Value =  Money.Coins(valor);
            return sucesso;
       }
       public void RetornaValor()
       {
           //mostro o valor atual
              MessageBox.Show("Saldo atual: "+bloco.Transactions[0].TotalOut.ToDecimal(MoneyUnit.BTC).ToString());
       }
      
    }
}
