import React, { useState } from "react";
import axios from "axios";
import { useNavigate } from "react-router-dom";

const LoginPage = () => {
  const [username, setUsername] = useState("");
  const [password, setPassword] = useState("");
  const [error, setError] = useState("");

  const navigate = useNavigate();

  const handleLogin = async () => {
    if (!username || !password) {
      showError("Lütfen kullanıcı adı ve şifre giriniz.");
      return;
    }
  
    try {
      const response = await axios.post("http://localhost:5139/api/Login/login", {
        username,
        password,
      });
  
      if (response.data.success) {
        const { storeId } = response.data; // Backend'den dönen StoreId
        navigate(`/admin-panel`); // Kullanıcının bağlı olduğu mağazaya yönlendir
      } else {
        showError(response.data.message || "Kullanıcı adı veya şifre hatalı.");
      }
    } catch (err) {
      // Backend'den gelen hata mesajını kontrol et
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
            type="text"
            placeholder="User name"
            value={username}
            onChange={(e) => setUsername(e.target.value)}
            style={styles.input}
          />
        </div>
        <div style={styles.inputGroup}>
          <input
            type="password"
            placeholder="Password"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            style={styles.input2}
          />
        </div>
        {error && <p style={styles.error}>{error}</p>}
        <button onClick={handleLogin} style={styles.button}>
          LOGIN
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
    height: "50%",
    backgroundColor: "#f3f4f6",
  },
  loginBox: {
    backgroundColor: "#fff",
    padding: "30px",
    borderRadius: "10px",
    boxShadow: "0 4px 8px rgba(0,0,0,0.1)",
    width: "100%",
    textAlign: "center",
  },
  title: {
    marginBottom: "20px",
    fontSize: "1.5rem",
    fontWeight: "600",
  },
  inputGroup: {
    marginBottom: "15px",
  },
  input: {
    width: "100%",
    padding: "10px",
    fontSize: "1rem",
    border: "1px solid #ddd",
    borderRadius: "5px",
  },
  input2: {
    width: "90%",
    padding: "10px",
    fontSize: "1rem",
    border: "1px solid #ddd",
    borderRadius: "5px",
  },
  button: {
    width: "50%",
    padding: "10px",
    backgroundColor: "#1c45b0",
    color: "#fff",
    fontSize: "1rem",
    fontWeight: "600",
    border: "none",
    borderRadius: "5px",
    cursor: "pointer",
    transition: "background-color 0.3s",
  },
  error: {
    color: "red",
    marginBottom: "10px",
  },
};

export default LoginPage;
