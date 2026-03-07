"use client";

import { useRef, useState, useCallback } from "react";
import { Canvas, useThree, type ThreeEvent } from "@react-three/fiber";
import { OrbitControls, RoundedBox, Text, Environment } from "@react-three/drei";
import * as THREE from "three";

interface BinData {
  id: string;
  row: number;
  col: number;
  hasComponents: boolean;
  label?: string;
}

interface StorageUnit3DViewerProps {
  rows: number;
  columns: number;
  bins: BinData[];
  selectedBin: { row: number; col: number } | null;
  onBinSelect: (row: number, col: number) => void;
}

// Bin dimensions (relative units)
const BIN_WIDTH = 1.0;
const BIN_HEIGHT = 0.7;
const BIN_DEPTH = 1.2;
const GAP = 0.08;

// Colors
const COLORS = {
  empty: "#94A3B8",
  emptyHover: "#B0BEC5",
  occupied: "#60A5FA",
  occupiedHover: "#93C5FD",
  selected: "#4F6EF7",
  selectedGlow: "#818CF8",
  shelf: "#334155",
  label: "#ffffff",
};

function Bin({
  position,
  row,
  col,
  hasComponents,
  isSelected,
  label,
  onSelect,
}: {
  position: [number, number, number];
  row: number;
  col: number;
  hasComponents: boolean;
  isSelected: boolean;
  label: string;
  onSelect: (row: number, col: number) => void;
}) {
  const meshRef = useRef<THREE.Mesh>(null);
  const [hovered, setHovered] = useState(false);

  const color = isSelected
    ? COLORS.selected
    : hovered
      ? hasComponents
        ? COLORS.occupiedHover
        : COLORS.emptyHover
      : hasComponents
        ? COLORS.occupied
        : COLORS.empty;

  const handleClick = useCallback(
    (e: ThreeEvent<MouseEvent>) => {
      e.stopPropagation();
      onSelect(row, col);
    },
    [onSelect, row, col]
  );

  return (
    <group position={position}>
      {/* Main bin body */}
      <RoundedBox
        ref={meshRef}
        args={[BIN_WIDTH, BIN_HEIGHT, BIN_DEPTH]}
        radius={0.03}
        smoothness={4}
        onClick={handleClick}
        onPointerOver={(e) => {
          e.stopPropagation();
          setHovered(true);
          document.body.style.cursor = "pointer";
        }}
        onPointerOut={() => {
          setHovered(false);
          document.body.style.cursor = "auto";
        }}
      >
        <meshStandardMaterial
          color={color}
          transparent
          opacity={isSelected ? 1 : 0.85}
          roughness={0.3}
          metalness={0.1}
        />
      </RoundedBox>

      {/* Front face label */}
      <Text
        position={[0, 0, BIN_DEPTH / 2 + 0.001]}
        fontSize={0.15}
        color={COLORS.label}
        anchorX="center"
        anchorY="middle"
        font={undefined}
      >
        {label}
      </Text>

      {/* Selection glow */}
      {isSelected && (
        <RoundedBox
          args={[BIN_WIDTH + 0.06, BIN_HEIGHT + 0.06, BIN_DEPTH + 0.06]}
          radius={0.04}
          smoothness={4}
        >
          <meshStandardMaterial
            color={COLORS.selectedGlow}
            transparent
            opacity={0.2}
            roughness={1}
          />
        </RoundedBox>
      )}

      {/* Drawer handle detail */}
      <mesh position={[0, -0.05, BIN_DEPTH / 2 + 0.02]}>
        <boxGeometry args={[0.3, 0.04, 0.02]} />
        <meshStandardMaterial
          color={isSelected ? "#c7d2fe" : "#cbd5e1"}
          roughness={0.2}
          metalness={0.4}
        />
      </mesh>
    </group>
  );
}

function ShelfUnit({
  rows,
  columns,
  bins,
  selectedBin,
  onBinSelect,
}: StorageUnit3DViewerProps) {
  const totalWidth = columns * (BIN_WIDTH + GAP) - GAP;
  const totalHeight = rows * (BIN_HEIGHT + GAP) - GAP;
  const offsetX = -totalWidth / 2 + BIN_WIDTH / 2;
  const offsetY = -totalHeight / 2 + BIN_HEIGHT / 2;

  const binMap = new Map<string, BinData>();
  for (const bin of bins) {
    binMap.set(`${bin.row}-${bin.col}`, bin);
  }

  return (
    <group>
      {/* Back panel */}
      <mesh position={[0, 0, -BIN_DEPTH / 2 - 0.05]}>
        <boxGeometry args={[totalWidth + 0.3, totalHeight + 0.3, 0.05]} />
        <meshStandardMaterial color={COLORS.shelf} roughness={0.8} />
      </mesh>

      {/* Side panels */}
      <mesh position={[-(totalWidth + 0.3) / 2, 0, 0]}>
        <boxGeometry args={[0.05, totalHeight + 0.3, BIN_DEPTH + 0.15]} />
        <meshStandardMaterial color={COLORS.shelf} roughness={0.8} />
      </mesh>
      <mesh position={[(totalWidth + 0.3) / 2, 0, 0]}>
        <boxGeometry args={[0.05, totalHeight + 0.3, BIN_DEPTH + 0.15]} />
        <meshStandardMaterial color={COLORS.shelf} roughness={0.8} />
      </mesh>

      {/* Bottom shelf */}
      <mesh position={[0, -(totalHeight + 0.3) / 2, 0]}>
        <boxGeometry args={[totalWidth + 0.3, 0.05, BIN_DEPTH + 0.15]} />
        <meshStandardMaterial color={COLORS.shelf} roughness={0.8} />
      </mesh>

      {/* Shelf dividers (horizontal) */}
      {Array.from({ length: rows - 1 }).map((_, r) => (
        <mesh
          key={`shelf-h-${r}`}
          position={[0, offsetY + (r + 1) * (BIN_HEIGHT + GAP) - GAP / 2 - BIN_HEIGHT / 2, 0]}
        >
          <boxGeometry args={[totalWidth + 0.2, 0.03, BIN_DEPTH + 0.1]} />
          <meshStandardMaterial color={COLORS.shelf} roughness={0.8} />
        </mesh>
      ))}

      {/* Bins */}
      {Array.from({ length: rows }).map((_, r) =>
        Array.from({ length: columns }).map((_, c) => {
          const binData = binMap.get(`${r}-${c}`);
          const x = offsetX + c * (BIN_WIDTH + GAP);
          const y = offsetY + (rows - 1 - r) * (BIN_HEIGHT + GAP);
          const label = `${String(r + 1).padStart(2, "0")}-${String(c + 1).padStart(2, "0")}`;

          return (
            <Bin
              key={`${r}-${c}`}
              position={[x, y, 0]}
              row={r}
              col={c}
              hasComponents={binData?.hasComponents ?? false}
              isSelected={
                selectedBin?.row === r && selectedBin?.col === c
              }
              label={label}
              onSelect={onBinSelect}
            />
          );
        })
      )}
    </group>
  );
}

function CameraSetup({ rows, columns }: { rows: number; columns: number }) {
  const { camera } = useThree();
  const distance = Math.max(rows, columns) * 1.2 + 2;

  if (camera instanceof THREE.PerspectiveCamera) {
    camera.position.set(distance * 0.6, distance * 0.3, distance * 0.8);
    camera.lookAt(0, 0, 0);
  }

  return null;
}

export function StorageUnit3DViewer({
  rows,
  columns,
  bins,
  selectedBin,
  onBinSelect,
}: StorageUnit3DViewerProps) {
  return (
    <div className="h-[500px] w-full rounded-xl border bg-gradient-to-b from-slate-900 to-slate-800 overflow-hidden">
      <Canvas
        camera={{ fov: 45, near: 0.1, far: 100 }}
        shadows
      >
        <CameraSetup rows={rows} columns={columns} />
        <ambientLight intensity={0.5} />
        <directionalLight position={[5, 8, 5]} intensity={0.8} />
        <directionalLight position={[-3, 4, -2]} intensity={0.3} />
        <pointLight position={[0, 0, 5]} intensity={0.4} />

        <ShelfUnit
          rows={rows}
          columns={columns}
          bins={bins}
          selectedBin={selectedBin}
          onBinSelect={onBinSelect}
        />

        <OrbitControls
          enablePan
          enableZoom
          enableRotate
          minDistance={2}
          maxDistance={20}
          maxPolarAngle={Math.PI / 1.5}
        />
        <Environment preset="city" />
      </Canvas>
    </div>
  );
}
