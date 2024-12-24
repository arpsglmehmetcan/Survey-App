import React, { useState } from "react";
import axios from "axios";
import { useNavigate } from "react-router-dom";

const LoginPage = () => {
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [error, setError] = useState("");

  const navigate = useNavigate();

  const handleLogin = async () => {
    if (!email || !password) {
      showError("Lütfen e-posta ve şifre giriniz.");
      return;
    }
  
    try {
      const response = await axios.post("http://localhost:5139/api/Login/login", {
        userMail: email,
        password,
      });
  
      if (response.data.success) {
        const { userMail, storeIds } = response.data; // Backend'den dönen userMail ve storeIds
  
        // localStorage'a kullanıcı verisini kaydedin
        localStorage.setItem(
          "userData",
          JSON.stringify({ userMail, storeIds })
        );
  
        // Kullanıcıyı AdminPanel'e yönlendir
        navigate(`/admin-panel`);
      } else {
        showError(response.data.message || "E-posta veya şifre hatalı.");
      }
    } catch (err) {
      if (err.response && err.response.data && err.response.data.message) {
        showError(err.response.data.message);
      } else {
        console.error("Giriş hatası:", err);
        showError("Bir hata oluştu. Lütfen tekrar deneyin.");
      }
    }
  };

  // Hata mesajını gösterip 3 saniye sonra silen fonksiyon
  const showError = (message) => {
    setError(message);
    setTimeout(() => {
      setError(""); // 3 saniye sonra hata mesajını temizle
    }, 3000);
  };

  return (
    <div style={styles.container}>
      <div style={styles.loginBox}>
        <h2 style={styles.title}>Admin Paneli Giriş</h2>
        <div style={styles.inputGroup}>
          <input
            type="email"
            placeholder="E-posta"
            value={email}
            onChange={(e) => setEmail(e.target.value)}
            style={styles.input}
          />
        </div>
        <div style={styles.inputGroup}>
          <input
            type="password"
            placeholder="Şifre"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            style={styles.input2}
          />
        </div>
        {error && <p style={styles.error}>{error}</p>}
        <button onClick={handleLogin} style={styles.button}>
          GİRİŞ YAP
        </button>
      </div>
    </div>
  );
};

// Stil ayarları
const styles = {
  container: {
    display: "flex",
    justifyContent: "center",
    alignItems: "center",
    height: "50vh", 
  },
  loginBox: {
    backgroundColor: "#fff",
    padding: "4%", 
    borderRadius: "15px", 
    boxShadow: "0 1px 16px rgba(0,0,0,0.2)", 
    width: "350px", 
    textAlign: "center",
  },
  title: {
    marginBottom: "20px",
    fontSize: "1.8rem", 
    fontWeight: "600",
    color: "#1c45b0",
  },
  inputGroup: {
    marginBottom: "15px",
  },
  input: {
    width: "100%",
    padding: "12px", 
    fontSize: "1.1rem", 
    border: "1px solid #ddd",
    borderRadius: "8px", 
    boxSizing: "border-box",
  },
  input2: {
    width: "100%",
    padding: "12px",
    fontSize: "1.1rem",
    border: "1px solid #ddd",
    borderRadius: "8px",
    boxSizing: "border-box",
  },
  button: {
    width: "60%", 
    padding: "12px", 
    backgroundColor: "#1c45b0",
    color: "#fff",
    fontSize: "1.2rem", 
    fontWeight: "700",
    border: "none",
    borderRadius: "8px",
    cursor: "pointer",
    transition: "background-color 0.3s",
  },
  error: {
    color: "red",
    marginBottom: "10px",
    fontSize: "1rem",
  },
};


export default LoginPage;
