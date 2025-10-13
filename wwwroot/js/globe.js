window.addEventListener("DOMContentLoaded", () => {
    const canvas = document.getElementById("earthCanvas");
    if (!canvas) return;

    const scene = new THREE.Scene();
    const camera = new THREE.PerspectiveCamera(45, 1, 0.1, 1000);
    const renderer = new THREE.WebGLRenderer({ canvas, alpha: true });
    renderer.setSize(canvas.clientWidth, canvas.clientHeight);

    const textureLoader = new THREE.TextureLoader();
    const earthTexture = textureLoader.load("https://threejs.org/examples/textures/land_ocean_ice_cloud_2048.jpg");
    const geometry = new THREE.SphereGeometry(1, 64, 64);
    const material = new THREE.MeshPhongMaterial({ map: earthTexture });
    const earth = new THREE.Mesh(geometry, material);
    scene.add(earth);

    const light = new THREE.PointLight(0xffffff, 1.5);
    light.position.set(5, 3, 5);
    scene.add(light);

    camera.position.z = 3;

    function animate() {
        requestAnimationFrame(animate);
        earth.rotation.y += 0.003; // rotation speed
        renderer.render(scene, camera);
    }
    animate();
});