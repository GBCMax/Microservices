import { Chart, registerables } from "chart.js"
import { useEffect, useState } from "react";
import { Line } from "react-chartjs-2";
import axios from "axios";

Chart.register(...registerables)

interface IResponse {
  speedValue: number;
}

const Home = () => {
  const [data, setData] = useState<number[]>([]);
  const [labels, setLabels] = useState<string[]>([]);
  const [count, setCount] = useState<number>(0);

  // Функция для получения данных из API
  const fetchData = async () => {
    try {
      const response = await axios.get('https://localhost:7186/api/WebSocket/connectToCar');
      const newData = response.data.speedValue;
      setCount((prev) => prev + 1);
      setData((prev) => [...prev, newData]);
      setLabels((prev) => [...prev, `Point ${count + 1}`]);
    } catch (error) {
      console.error('Ошибка при получении данных:', error);
    }
  };

  useEffect(() => {
    const interval = setInterval(fetchData, 1000); // Каждые 1 секунду запрашиваем данные из API

    return () => clearInterval(interval); // Очистка интервала при размонтировании
  }, []);

  // Данные для графика
  const chartData = {
    labels: labels,
    datasets: [
      {
        label: 'Скорость (km/h)',
        data: data,
        fill: false,
        borderColor: 'rgba(75, 192, 192, 1)',
        tension: 0.1,
      },
    ],
  };

    return (
      <div className="flex 
        flex-col 
        items-center 
        place-content-center 
        h-dvh "
      >
        <Line data={chartData} />
      </div>
    )
  }
  
  export default Home